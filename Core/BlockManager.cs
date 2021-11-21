using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MaxMind.GeoIP2;
using Microsoft.AspNetCore.Http;
using SSCMS.Block.Abstractions;
using SSCMS.Block.Models;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Block.Core
{
    public class BlockManager : IBlockManager
    {
        public const string PluginId = "sscms.block";

        private readonly ISettingsManager _settingsManager;
        private readonly IAnalysisRepository _analysisRepository;
        private readonly IRuleRepository _ruleRepository;

        private static DatabaseReader _reader;
        private static List<Area> _areas;

        public BlockManager(ISettingsManager settingsManager, IPluginManager pluginManager, IAnalysisRepository analysisRepository, IRuleRepository ruleRepository)
        {
            _settingsManager = settingsManager;
            _analysisRepository = analysisRepository;
            _ruleRepository = ruleRepository;

            var plugin = pluginManager.GetPlugin(PluginId);

            if (_areas == null)
            {
                _areas = new List<Area>();

                var locationsEn =
                    PathUtils.Combine(plugin.WebRootPath,
                        "assets/block/GeoLite2-Country-CSV_20190423/GeoLite2-Country-Locations-en.csv");
                var locationsCn =
                    PathUtils.Combine(plugin.WebRootPath,
                        "assets/block/GeoLite2-Country-CSV_20190423/GeoLite2-Country-Locations-zh-CN.csv");
                var enCsv = File.ReadAllLines(locationsEn);
                var cnCsv = File.ReadAllLines(locationsCn);

                for (var i = 0; i < enCsv.Length; i++)
                {
                    if (i == 0) continue;

                    var enSplits = enCsv[i].Split(',');
                    var cnSplits = cnCsv[i].Split(',');

                    var geoNameIdEn = TranslateUtils.ToInt(enSplits[0]);
                    var areaEn = enSplits[5].Trim('"');
                    var geoNameIdCn = TranslateUtils.ToInt(cnSplits[0]);
                    var areaCn = cnSplits[5].Trim('"');

                    if (geoNameIdEn == geoNameIdCn && !string.IsNullOrEmpty(areaEn) && !string.IsNullOrEmpty(areaCn))
                    {
                        _areas.Add(new Area
                        {
                            GeoNameId = geoNameIdEn,
                            AreaEn = areaEn,
                            AreaCn = areaCn
                        });
                    }
                }

                _areas = _areas.OrderBy(x => x.AreaEn).ToList();

                _areas.Insert(0, new Area
                {
                    GeoNameId = LocalGeoNameId,
                    AreaEn = LocalAreaEn,
                    AreaCn = LocalAreaCn
                });
            }

            if (_reader == null)
            {
                var filePath = PathUtils.Combine(plugin.WebRootPath,
                    "assets/block/GeoLite2-Country_20190423/GeoLite2-Country.mmdb");
                _reader = new DatabaseReader(filePath);
            }
        }

        public List<IdName> GetAreas()
        {
            return _areas.Select(x => new IdName
                {
                    Id = x.GeoNameId,
                    Name = $"{x.AreaEn}({x.AreaCn})"
                })
                .ToList();
        }

        public Area GetArea(int geoNameId)
        {
            return _areas.FirstOrDefault(x => x.GeoNameId == geoNameId);
        }

        public int GetGeoNameId(string ipAddress)
        {
            if (IsLocalIp(ipAddress)) return LocalGeoNameId;
            return _reader.TryCountry(ipAddress, out var response) ? (int) response.Country.GeoNameId : 0;
        }

        private static bool IsLocalIp(string ipAddress)
        {
            return ipAddress == "127.0.0.1" || Regex.IsMatch(ipAddress,
                @"(^192\.168\.([0-9]|[0-9][0-9]|[0-2][0-5][0-5])\.([0-9]|[0-9][0-9]|[0-2][0-5][0-5])$)|(^172\.([1][6-9]|[2][0-9]|[3][0-1])\.([0-9]|[0-9][0-9]|[0-2][0-5][0-5])\.([0-9]|[0-9][0-9]|[0-2][0-5][0-5])$)|(^10\.([0-9]|[0-9][0-9]|[0-2][0-5][0-5])\.([0-9]|[0-9][0-9]|[0-2][0-5][0-5])\.([0-9]|[0-9][0-9]|[0-2][0-5][0-5])$)");
        }

        private static void AddRestrictionToIpList(IpList list, string restriction)
        {
            if (string.IsNullOrEmpty(restriction)) return;

            if (StringUtils.Contains(restriction, "-"))
            {
                restriction = restriction.Trim(' ', '-');
                var arr = restriction.Split('-');
                list.AddRange(arr[0].Trim(), arr[1].Trim());
            }
            else if (StringUtils.Contains(restriction, "*"))
            {
                var ipPrefix = restriction.Substring(0, restriction.IndexOf('*'));
                ipPrefix = ipPrefix.Trim(' ', '.');
                var dotNum = StringUtils.GetCount(".", ipPrefix);

                string ipNumber;
                string mask;
                if (dotNum == 0)
                {
                    ipNumber = ipPrefix + ".0.0.0";
                    mask = "255.0.0.0";
                }
                else if (dotNum == 1)
                {
                    ipNumber = ipPrefix + ".0.0";
                    mask = "255.255.0.0";
                }
                else
                {
                    ipNumber = ipPrefix + ".0";
                    mask = "255.255.255.0";
                }
                list.Add(ipNumber, mask);
            }
            else
            {
                list.Add(restriction);
            }
        }

        private static bool IsAllowed(string ipAddress, List<string> blockList, List<string> allowList)
        {
            var isAllowed = true;

            if (blockList != null && blockList.Count > 0)
            {
                var list = new IpList();
                foreach (var restriction in blockList)
                {
                    AddRestrictionToIpList(list, restriction);
                }
                if (list.CheckNumber(ipAddress))
                {
                    isAllowed = false;
                }
            }
            else if (allowList != null && allowList.Count > 0)
            {
                isAllowed = false;
                var list = new IpList();
                foreach (var restriction in allowList)
                {
                    AddRestrictionToIpList(list, restriction);
                }
                if (list.CheckNumber(ipAddress))
                {
                    isAllowed = true;
                }
            }

            return isAllowed;
        }

        public async Task<(bool, Rule)> IsBlockedAsync(int siteId, string ipAddress, string sessionId)
        {
            var rules = await _ruleRepository.GetAllAsync(siteId);
            if (rules == null || rules.Count == 0) return (false, null);

            var geoNameId = GetGeoNameId(ipAddress);
            var area = GetArea(geoNameId);

            foreach (var rule in rules)
            {
                if (rule.BlockMethod == BlockMethod.Password && !string.IsNullOrEmpty(sessionId))
                {
                    if (rule.Password == _settingsManager.Decrypt(sessionId))
                    {
                        continue;
                    }
                }

                var isMatch = false;
                if (area != null)
                {
                    if (rule.BlockAreas != null && rule.BlockAreas.Contains(area.GeoNameId))
                    {
                        isMatch = true;
                    }
                }

                var isBlocked = false;
                if (rule.AreaType == AreaType.Includes)
                {
                    isBlocked = !isMatch;
                }
                else if (rule.AreaType == AreaType.Excludes)
                {
                    isBlocked = isMatch;
                }

                if (!isBlocked)
                {
                    isBlocked = !IsAllowed(ipAddress, rule.BlockList, rule.AllowList);
                }

                if (isBlocked)
                {
                    await _analysisRepository.AddBlockAsync(siteId);
                    return (true, rule);
                }
            }

            return (false, null);
        }

        public const string PermissionsAnalysis = "block_analysis";
        public const string PermissionsQuery = "block_query";
        public const string PermissionsSettings = "block_settings";

        public const int LocalGeoNameId = 10000;

        public const string LocalAreaEn = "Local IP";

        public const string LocalAreaCn = "内网地址";

        private static bool IsIpAddress(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }

        public static string GetIpAddress(HttpRequest request)
        {
            var result = string.Empty;

            try
            {
                result = request.Headers["HTTP_X_FORWARDED_FOR"];
                if (!string.IsNullOrEmpty(result))
                {
                    if (result.IndexOf(".", StringComparison.Ordinal) == -1)
                        result = null;
                    else
                    {
                        if (result.IndexOf(",", StringComparison.Ordinal) != -1)
                        {
                            result = result.Replace("  ", "").Replace("'", "");
                            var temporary = result.Split(",;".ToCharArray());
                            foreach (var t in temporary)
                            {
                                if (IsIpAddress(t) && t.Substring(0, 3) != "10." && t.Substring(0, 7) != "192.168" && t.Substring(0, 7) != "172.16.")
                                {
                                    result = t;
                                }
                            }
                            var str = result.Split(',');
                            if (str.Length > 0)
                                result = str[0].Trim();
                        }
                        else if (IsIpAddress(result))
                            return result;
                    }
                }

                if (string.IsNullOrEmpty(result))
                {
                    result = request.Headers["REMOTE_ADDR"];
                }

                if (string.IsNullOrEmpty(result))
                {
                    result = request.HttpContext.Connection.RemoteIpAddress.ToString();
                }

                if (string.IsNullOrEmpty(result) || result == "::1")
                {
                    result = "127.0.0.1";
                }
            }
            catch
            {
                // ignored
            }

            return result;
        }
    }
}

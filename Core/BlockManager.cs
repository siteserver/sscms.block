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
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Block.Core
{
    public class BlockManager : IBlockManager
    {
        private readonly ISettingsManager _settingsManager;
        private readonly IPlugin _plugin;
        private readonly IPluginConfigRepository _pluginConfigRepository;
        private readonly IBlockRepository _blockRepository;

        private static DatabaseReader _reader;
        private static List<Area> _areas;


        public BlockManager(ISettingsManager settingsManager, IPluginManager pluginManager, IPluginConfigRepository pluginConfigRepository, IBlockRepository blockRepository)
        {
            _settingsManager = settingsManager;
            _pluginConfigRepository = pluginConfigRepository;
            _blockRepository = blockRepository;

            _plugin = pluginManager.Current;

            if (_areas == null)
            {
                _areas = new List<Area>();

                var locationsEn =
                    PathUtils.Combine(_plugin.WebRootPath,
                        "assets/block/GeoLite2-Country-CSV_20190423/GeoLite2-Country-Locations-en.csv");
                var locationsCn =
                    PathUtils.Combine(_plugin.WebRootPath,
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
                var filePath = PathUtils.Combine(_plugin.WebRootPath,
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

        public async Task<bool> IsAllowedAsync(int siteId, Config config, Area area, string sessionId)
        {
            if (!config.IsEnabled) return true;

            if (config.BlockMethod == nameof(config.Password) && !string.IsNullOrEmpty(sessionId))
            {
                if (config.Password == _settingsManager.Decrypt(sessionId))
                {
                    return true;
                }
            }
            
            var isMatch = false;
            if (area != null)
            {
                if (config.BlockAreas != null && config.BlockAreas.Contains(area.GeoNameId))
                {
                    isMatch = true;
                }
            }

            bool isAllowed;
            if (config.IsAllAreas)
            {
                isAllowed = isMatch;
            }
            else
            {
                isAllowed = !isMatch;
            }

            if (!isAllowed)
            {
                await _blockRepository.AddBlockAsync(siteId);
            }

            return isAllowed;
        }

        public const string PermissionsAnalysis = "block_analysis";
        public const string PermissionsIp = "block_ip";
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

        public async Task<Config> GetConfigAsync(int siteId)
        {
            var pluginId = _plugin.PluginId;
            return await _pluginConfigRepository.GetConfigAsync<Config>(pluginId, siteId) ?? new Config();
        }

        public async Task<bool> SetConfigAsync(int siteId, Config config)
        {
            var pluginId = _plugin.PluginId;
            return await _pluginConfigRepository.SetConfigAsync(pluginId, siteId, config);
        }
    }
}

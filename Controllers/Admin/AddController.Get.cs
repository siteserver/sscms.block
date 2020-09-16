using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Block.Core;
using SSCMS.Block.Models;
using SSCMS.Utils;

namespace SSCMS.Block.Controllers.Admin
{
    public partial class AddController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, BlockManager.PermissionsSettings))
            {
                return Unauthorized();
            }

            var rule = await _ruleRepository.GetAsync(request.SiteId, request.RuleId) ?? new Rule
            {
                SiteId = request.SiteId,
                AreaType = AreaType.None,
                AllowList = new List<string>(),
                BlockList = new List<string>(),
                BlockMethod = BlockMethod.Warning
            };

            var areas = _blockManager.GetAreas();
            var blockAreas = new List<IdName>();
            if (rule.BlockAreas != null && areas != null)
            {
                blockAreas = areas.Where(x => rule.BlockAreas.Contains(x.Id)).ToList();
            }

            var channels = new List<IdName>();
            var channelIdList = await _channelRepository.GetChannelIdsAsync(request.SiteId);
            foreach (var channelId in channelIdList)
            {
                channels.Add(new IdName
                {
                    Id = channelId,
                    Name = await _channelRepository.GetChannelNameNavigationAsync(request.SiteId, channelId)
                });
            }
            var blockChannels = new List<IdName>();
            if (rule.BlockChannels != null)
            {
                blockChannels = channels.Where(x => rule.BlockChannels.Contains(x.Id)).ToList();
            }

            return new GetResult
            {
                Rule = rule,
                AreaTypes = ListUtils.GetSelects<AreaType>(),
                BlockAreas = blockAreas,
                BlockChannels = blockChannels
            };
        }
    }
}

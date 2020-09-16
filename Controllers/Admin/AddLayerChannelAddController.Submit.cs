using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Block.Core;
using SSCMS.Block.Models;

namespace SSCMS.Block.Controllers.Admin
{
    public partial class AddLayerChannelAddController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<SubmitResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, BlockManager.PermissionsSettings))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channels = new List<IdName>();
            foreach (var channelId in request.ChannelIds)
            {
                var name = await _channelRepository.GetChannelNameNavigationAsync(request.SiteId, channelId);

                channels.Add(new IdName
                {
                    Id = channelId,
                    Name = name
                });
            }

            return new SubmitResult
            {
                Channels = channels
            };
        }
    }
}
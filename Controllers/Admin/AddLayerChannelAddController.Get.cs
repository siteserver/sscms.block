using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Block.Core;
using SSCMS.Dto;

namespace SSCMS.Block.Controllers.Admin
{
    public partial class AddLayerChannelAddController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, BlockManager.PermissionsSettings))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var transChannels = await _channelRepository.GetAsync(request.SiteId);
            var transSite = await _siteRepository.GetAsync(request.SiteId);
            var cascade = await _channelRepository.GetCascadeAsync(transSite, transChannels);

            return new GetResult
            {
                Channels = cascade
            };
        }
    }
}
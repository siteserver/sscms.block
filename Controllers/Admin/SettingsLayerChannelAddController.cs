using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Block.Implements;
using SSCMS.Block.Models;
using SSCMS.Dto;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Block.Controllers.Admin
{
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class SettingsLayerChannelAddController : ControllerBase
    {
        private const string Route = "block/settingsLayerChannelAdd";

        private readonly IAuthManager _authManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;

        public SettingsLayerChannelAddController(IAuthManager authManager, ISiteRepository siteRepository, IChannelRepository channelRepository)
        {
            _authManager = authManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
        }

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

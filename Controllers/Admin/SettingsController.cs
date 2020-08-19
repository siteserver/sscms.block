using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Block.Abstractions;
using SSCMS.Block.Core;
using SSCMS.Block.Models;
using SSCMS.Dto;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Block.Controllers.Admin
{
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class SettingsController : ControllerBase
    {
        private const string Route = "block/settings";

        private readonly IAuthManager _authManager;
        private readonly IChannelRepository _channelRepository;
        private readonly IBlockManager _blockManager;

        public SettingsController(IAuthManager authManager, IChannelRepository channelRepository, IBlockManager blockManager)
        {
            _authManager = authManager;
            _channelRepository = channelRepository;
            _blockManager = blockManager;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> GetConfig([FromQuery] GetRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, BlockManager.PermissionsSettings))
            {
                return Unauthorized();
            }

            var config = await _blockManager.GetConfigAsync(request.SiteId);

            var areas = _blockManager.GetAreas();
            var blockAreas = new List<IdName>();
            if (config.BlockAreas != null && areas != null)
            {
                blockAreas = areas.Where(x => config.BlockAreas.Contains(x.Id)).ToList();
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
            if (config.BlockChannels != null)
            {
                blockChannels = channels.Where(x => config.BlockChannels.Contains(x.Id)).ToList();
            }

            return new GetResult
            {
                Config = config,
                BlockAreas = blockAreas,
                BlockChannels = blockChannels
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> SetConfig([FromBody] SetRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, BlockManager.PermissionsSettings))
            {
                return Unauthorized();
            }

            var config = request.Config;
            config.BlockAreas = request.BlockAreas.Select(x => x.Id).ToList();
            config.BlockChannels = request.BlockChannels.Select(x => x.Id).ToList();

            await _blockManager.SetConfigAsync(request.SiteId, request.Config);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}

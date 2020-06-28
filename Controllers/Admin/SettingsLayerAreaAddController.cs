using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Block.Abstractions;
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
    public partial class SettingsLayerAreaAddController : ControllerBase
    {
        private const string Route = "block/settingsLayerAreaAdd";

        private readonly IAuthManager _authManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IBlockManager _blockManager;

        public SettingsLayerAreaAddController(IAuthManager authManager, ISiteRepository siteRepository, IBlockManager blockManager)
        {
            _authManager = authManager;
            _siteRepository = siteRepository;
            _blockManager = blockManager;
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

            var areas = _blockManager.GetAreas();

            return new GetResult
            {
                Areas = areas.Select(x => new Select<int>
                {
                    Value = x.Id,
                    Label = x.Name
                }).ToList()
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

            var areas = new List<IdName>();
            foreach (var areaId in request.AreaIds)
            {
                var area = _blockManager.GetArea(areaId);

                areas.Add(new IdName
                {
                    Id = areaId,
                    Name = $"{area.AreaEn}({area.AreaCn})"
                });
            }

            return new SubmitResult
            {
                Areas = areas
            };
        }
    }
}

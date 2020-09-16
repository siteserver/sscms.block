using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Block.Core;
using SSCMS.Dto;

namespace SSCMS.Block.Controllers.Admin
{
    public partial class AddLayerAreaAddController
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
    }
}
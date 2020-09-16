using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Block.Core;
using SSCMS.Block.Models;

namespace SSCMS.Block.Controllers.Admin
{
    public partial class AddLayerAreaAddController
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
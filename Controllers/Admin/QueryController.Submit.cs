using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Block.Core;

namespace SSCMS.Block.Controllers.Admin
{
    public partial class QueryController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<SubmitResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, BlockManager.PermissionsQuery))
            {
                return Unauthorized();
            }

            var (isBlocked, _) = await _blockManager.IsBlockedAsync(request.SiteId, request.IpAddress, string.Empty);

            var geoNameId = _blockManager.GetGeoNameId(request.IpAddress);
            var area = _blockManager.GetArea(geoNameId);

            return new SubmitResult
            {
                IsAllowed = !isBlocked,
                Area = area
            };
        }
    }
}

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Block.Core;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Block.Controllers.Admin
{
    public partial class QueryController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<StringResult>> Get([FromQuery] GetRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, BlockManager.PermissionsQuery))
            {
                return Unauthorized();
            }

            return new StringResult
            {
                Value = PageUtils.GetIpAddress(Request)
            };
        }
    }
}

using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Block.Abstractions;
using SSCMS.Block.Implements;
using SSCMS.Dto;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Block.Controllers.Admin
{
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class IpController : ControllerBase
    {
        private const string Route = "block/ip";

        private readonly IAuthManager _authManager;
        private readonly IBlockManager _blockManager;

        public IpController(IAuthManager authManager, IBlockManager blockManager)
        {
            _authManager = authManager;
            _blockManager = blockManager;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<StringResult>> GetIpAddress([FromQuery] GetRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, BlockManager.PermissionsIp))
            {
                return Unauthorized();
            }

            return new StringResult
            {
                Value = BlockManager.GetIpAddress(Request)
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<QueryResult>> Query([FromBody] QueryRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, BlockManager.PermissionsIp))
            {
                return Unauthorized();
            }

            var config = await _blockManager.GetConfigAsync(request.SiteId);

            var geoNameId = _blockManager.GetGeoNameId(request.IpAddress);
            var areaInfo = _blockManager.GetArea(geoNameId);
            var isAllowed = await _blockManager.IsAllowedAsync(request.SiteId, config, areaInfo, string.Empty);

            return new QueryResult
            {
                IsAllowed = isAllowed,
                Area = areaInfo
            };
        }
    }
}

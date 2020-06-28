using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Block.Abstractions;
using SSCMS.Block.Implements;
using SSCMS.Services;

namespace SSCMS.Block.Controllers
{
    [Route("api/block")]
    public partial class BlockController : ControllerBase
    {
        private const string Route = "";

        private readonly ISettingsManager _settingsManager;
        private readonly IBlockManager _blockManager;

        public BlockController(ISettingsManager settingsManager, IBlockManager blockManager)
        {
            _settingsManager = settingsManager;
            _blockManager = blockManager;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<QueryResult>> Query([FromQuery] QueryRequest request)
        {
            var config = await _blockManager.GetConfigAsync(request.SiteId);

            var ipAddress = BlockManager.GetIpAddress(Request);
            var geoNameId = _blockManager.GetGeoNameId(ipAddress);
            var areaInfo = _blockManager.GetArea(geoNameId);
            var isAllowed = await _blockManager.IsAllowedAsync(request.SiteId, config, areaInfo, request.SessionId);
            var blockMethod = config.BlockMethod;
            var redirectUrl = config.RedirectUrl;
            var warning = config.Warning;

            return new QueryResult
            {
                IsAllowed = isAllowed,
                BlockMethod = blockMethod,
                RedirectUrl = redirectUrl,
                Warning = warning
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<AuthResult>> Auth([FromBody] AuthRequest request)
        {
            var config = await _blockManager.GetConfigAsync(request.SiteId);

            var sessionId = string.Empty;
            if (config.Password == request.Password)
            {
                sessionId = _settingsManager.Encrypt(request.Password);
            }

            return new AuthResult
            {
                Success = config.Password == request.Password,
                SessionId = sessionId
            };
        }
    }
}

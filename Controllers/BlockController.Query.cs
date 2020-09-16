using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Block.Core;
using SSCMS.Block.Models;

namespace SSCMS.Block.Controllers
{
    public partial class BlockController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<QueryResult>> Query([FromQuery] QueryRequest request)
        {
            var ipAddress = BlockManager.GetIpAddress(Request);
            var (isBlocked, rule) = await _blockManager.IsBlockedAsync(request.SiteId, ipAddress, request.SessionId);

            var blockMethod = BlockMethod.RedirectUrl;
            var redirectUrl = string.Empty;
            var warning = string.Empty;
            if (isBlocked)
            {
                blockMethod = rule.BlockMethod;
                redirectUrl = rule.RedirectUrl;
                warning = rule.Warning;
            }

            return new QueryResult
            {
                IsAllowed = !isBlocked,
                BlockMethod = blockMethod,
                RedirectUrl = redirectUrl,
                Warning = warning
            };
        }
    }
}

using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Block.Core;

namespace SSCMS.Block.Controllers.Admin
{
    public partial class AnalysisController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, BlockManager.PermissionsAnalysis))
            {
                return Unauthorized();
            }

            var blockedList = await _analysisRepository.GetMonthlyBlockedListAsync(request.SiteId);
            var labels = blockedList.Select(x => x.Key).ToList();
            var data = blockedList.Select(x => x.Value).ToList();

            return new GetResult
            {
                Days = labels,
                Count = data
            };
        }
    }
}

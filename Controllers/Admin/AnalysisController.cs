using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Block.Abstractions;
using SSCMS.Block.Implements;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Block.Controllers.Admin
{
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class AnalysisController : ControllerBase
    {
        private const string Route = "block/analysis";

        private readonly IAuthManager _authManager;
        private readonly IBlockRepository _blockRepository;

        public AnalysisController(IAuthManager authManager, IBlockRepository blockRepository)
        {
            _authManager = authManager;
            _blockRepository = blockRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> GetAnalysis([FromQuery] GetRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, BlockManager.PermissionsAnalysis))
            {
                return Unauthorized();
            }

            var blockedList = await _blockRepository.GetMonthlyBlockedListAsync(request.SiteId);
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

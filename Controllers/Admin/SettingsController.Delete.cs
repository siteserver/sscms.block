using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Block.Core;

namespace SSCMS.Block.Controllers.Admin
{
    public partial class SettingsController
    {
        [HttpPost, Route(RouteDelete)]
        public async Task<ActionResult<GetResult>> Delete([FromBody] DeleteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, BlockManager.PermissionsSettings))
            {
                return Unauthorized();
            }

            await _ruleRepository.DeleteAsync(request.SiteId, request.RuleId);

            var rules = await _ruleRepository.GetAllAsync(request.SiteId);

            foreach (var rule in rules)
            {
                rule.Set("channels", await GetChannelsAsync(request.SiteId, rule));
            }

            return new GetResult
            {
                Rules = rules
            };
        }
    }
}

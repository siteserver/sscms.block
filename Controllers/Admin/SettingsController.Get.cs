using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Block.Core;
using SSCMS.Dto;

namespace SSCMS.Block.Controllers.Admin
{
    public partial class SettingsController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, BlockManager.PermissionsSettings))
            {
                return Unauthorized();
            }

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

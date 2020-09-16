using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Block.Core;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Block.Controllers.Admin
{
    public partial class AddController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, BlockManager.PermissionsSettings))
            {
                return Unauthorized();
            }

            if (request.Rule.Id == 0)
            {
                if (await _ruleRepository.IsExistsAsync(request.SiteId, request.Rule.RuleName))
                {
                    return this.Error("保存失败，已存在相同名称的拦截规则！");
                }
            }

            var rule = request.Rule;
            rule.BlockAreas = request.BlockAreas.Select(x => x.Id).ToList();
            rule.BlockChannels = request.BlockChannels.Select(x => x.Id).ToList();

            if (rule.Id > 0)
            {
                await _ruleRepository.UpdateAsync(rule);
            }
            else
            {
                await _ruleRepository.InsertAsync(rule);
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}

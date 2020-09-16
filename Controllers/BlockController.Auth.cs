using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SSCMS.Block.Controllers
{
    public partial class BlockController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<AuthResult>> Auth([FromBody] AuthRequest request)
        {
            var sessionId = string.Empty;

            var rules = await _ruleRepository.GetAllAsync(request.SiteId);
            if (rules != null)
            {
                foreach (var rule in rules)
                {
                    if (rule.Password == request.Password)
                    {
                        sessionId = _settingsManager.Encrypt(rule.Password);
                        break;
                    }
                }
            }
            
            return new AuthResult
            {
                Success = !string.IsNullOrEmpty(sessionId),
                SessionId = sessionId
            };
        }
    }
}

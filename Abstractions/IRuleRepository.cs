using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Block.Models;

namespace SSCMS.Block.Abstractions
{
    public interface IRuleRepository
    {
        Task<Rule> GetAsync(int siteId, int ruleId);

        Task<int> InsertAsync(Rule rule);

        Task<bool> UpdateAsync(Rule rule);

        Task<bool> IsExistsAsync(int siteId, string ruleName);

        Task DeleteAsync(int siteId, int ruleId);

        Task<List<Rule>> GetAllAsync(int siteId);
    }
}

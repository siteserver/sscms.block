using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using SSCMS.Block.Abstractions;
using SSCMS.Block.Models;
using SSCMS.Services;

namespace SSCMS.Block.Core
{
    public class RuleRepository : IRuleRepository
    {
        private readonly Repository<Rule> _repository;

        public RuleRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<Rule>(new Database(settingsManager.DatabaseType, settingsManager.DatabaseConnectionString));
        }

        private static string GetCacheKey(int siteId)
        {
            return $"SSCMS.Block.Core:{siteId}";
        }

        public async Task<Rule> GetAsync(int siteId, int ruleId)
        {
            var rules = await GetAllAsync(siteId);
            return rules.FirstOrDefault(x => x.Id == ruleId);
        }

        public async Task<int> InsertAsync(Rule ad)
        {
            return await _repository.InsertAsync(ad, Q.CachingRemove(GetCacheKey(ad.SiteId)));
        }

        public async Task<bool> UpdateAsync(Rule ad)
        {
            return await _repository.UpdateAsync(ad, Q.CachingRemove(GetCacheKey(ad.SiteId)));
        }

        public async Task<bool> IsExistsAsync(int siteId, string ruleName)
        {
            var rules = await GetAllAsync(siteId);
            return rules.Exists(x => x.RuleName == ruleName);
        }

        public async Task DeleteAsync(int siteId, int ruleId)
        {
            await _repository.DeleteAsync(ruleId, Q.CachingRemove(GetCacheKey(siteId)));
        }

        public async Task<List<Rule>> GetAllAsync(int siteId)
        {
            return await _repository.GetAllAsync(Q
                .Where(nameof(Rule.SiteId), siteId)
                .OrderByDesc(nameof(Rule.Id))
                .CachingGet(GetCacheKey(siteId))
            );
        }
    }
}

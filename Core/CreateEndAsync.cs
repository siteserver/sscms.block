using System.Threading.Tasks;
using SSCMS.Block.Abstractions;
using SSCMS.Configuration;
using SSCMS.Parse;
using SSCMS.Plugins;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Block.Core
{
    public class CreateEndAsync : IPluginCreateEndAsync
    {
        private readonly IPathManager _pathManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IRuleRepository _ruleRepository;

        public CreateEndAsync(IPathManager pathManager, ISiteRepository siteRepository, IRuleRepository ruleRepository)
        {
            _pathManager = pathManager;
            _siteRepository = siteRepository;
            _ruleRepository = ruleRepository;
        }

        public async Task ParseAsync(IParseContext context)
        {
            var rules = await _ruleRepository.GetAllAsync(context.SiteId);
            if (rules == null || rules.Count == 0) return;

            var isChannel = false;
            foreach (var rule in rules)
            {
                if (rule.IsAllChannels)
                {
                    isChannel = true;
                    break;
                }

                if (rule.BlockChannels != null && rule.BlockChannels.Contains(context.ChannelId))
                {
                    isChannel = true;
                    break;
                }
            }

            if (!isChannel) return;

            var site = await _siteRepository.GetAsync(context.SiteId);

            var urlPrefix = _pathManager.GetApiHostUrl(site, "/assets/block");
            var apiUrl = _pathManager.GetApiHostUrl(site, Constants.ApiPrefix);

            context.HeadCodes[BlockManager.PluginId] = $@"
<style>body{{display: none;}}</style>
<script src=""{urlPrefix}/lib/es6-promise.auto.min.js"" type=""text/javascript""></script>
<script src=""{urlPrefix}/lib/axios-0.18.0.min.js"" type=""text/javascript""></script>
<script src=""{urlPrefix}/lib/sweetalert2-7.28.4.all.min.js"" type=""text/javascript""></script>
<script src=""{urlPrefix}/block.js"" data-api-url=""{apiUrl}"" data-site-id=""{context.SiteId}"" type=""text/javascript""></script>
";
        }
    }
}

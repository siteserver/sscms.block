using System.Threading.Tasks;
using SSCMS.Block.Abstractions;
using SSCMS.Parse;
using SSCMS.Plugins;
using SSCMS.Services;

namespace SSCMS.Block.Core
{
    public class CreateEndAsync : IPluginCreateEndAsync
    {
        private readonly IPathManager _pathManager;
        private readonly IBlockManager _blockManager;

        public CreateEndAsync(IPathManager pathManager, IBlockManager blockManager)
        {
            _pathManager = pathManager;
            _blockManager = blockManager;
        }

        public async Task ParseAsync(IParseContext context)
        {
            var config = await _blockManager.GetConfigAsync(context.SiteId);
            if (!config.IsEnabled) return;

            var isChannel = false;
            if (config.IsAllChannels)
            {
                isChannel = true;
            }
            else
            {
                if (config.BlockChannels != null && config.BlockChannels.Contains(context.ChannelId))
                {
                    isChannel = true;
                }
            }

            if (!isChannel) return;

            var urlPrefix = _pathManager.GetRootUrl("/assets/block");
            var apiUrl = _pathManager.GetApiUrl();

            context.HeadCodes[BlockManager.PluginId] = $@"
<style>body{{display: none !important}}</style>
<script src=""{urlPrefix}/lib/es6-promise.auto.min.js"" type=""text/javascript""></script>
<script src=""{urlPrefix}/lib/axios-0.18.0.min.js"" type=""text/javascript""></script>
<script src=""{urlPrefix}/lib/sweetalert2-7.28.4.all.min.js"" type=""text/javascript""></script>
<script src=""{urlPrefix}/block.js"" data-api-url=""{apiUrl}"" data-site-id=""{context.SiteId}"" type=""text/javascript""></script>
";
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using SSCMS.Block.Abstractions;
using SSCMS.Block.Core;
using SSCMS.Plugins;

namespace SSCMS.Block
{
    public class Startup : IPluginConfigureServices
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IBlockRepository, BlockRepository>();
            services.AddScoped<IBlockManager, BlockManager>();
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using SSCMS.Block.Abstractions;
using SSCMS.Plugins;

namespace SSCMS.Block.Implements
{
    public class PluginConfigureServices : IPluginConfigureServices
    {

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IBlockRepository, BlockRepository>();
            services.AddScoped<IBlockManager, BlockManager>();
        }
    }
}

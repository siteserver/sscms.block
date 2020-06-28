using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Block.Models;

namespace SSCMS.Block.Abstractions
{
    public interface IBlockManager
    {
        List<IdName> GetAreas();

        Area GetArea(int geoNameId);

        int GetGeoNameId(string ipAddress);

        Task<bool> IsAllowedAsync(int siteId, Config config, Area area, string sessionId);

        Task<Config> GetConfigAsync(int siteId);

        Task<bool> SetConfigAsync(int siteId, Config config);
    }
}

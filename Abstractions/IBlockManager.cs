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

        Task<(bool, Rule)> IsBlockedAsync(int siteId, string ipAddress, string sessionId);
    }
}

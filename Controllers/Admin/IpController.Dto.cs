using SSCMS.Block.Models;

namespace SSCMS.Block.Controllers.Admin
{
    public partial class IpController
    {
        public class GetRequest
        {
            public int SiteId { get; set; }
        }

        public class QueryRequest
        {
            public int SiteId { get; set; }
            public string IpAddress { get; set; }
        }

        public class QueryResult
        {
            public bool IsAllowed { get; set; }
            public Area Area { get; set; }
        }
    }
}

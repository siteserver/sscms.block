using System.Collections.Generic;

namespace SSCMS.Block.Controllers.Admin
{
    public partial class AnalysisController
    {
        public class GetRequest
        {
            public int SiteId { get; set; }
        }

        public class GetResult
        {
            public List<string> Days { get; set; }
            public List<int> Count { get; set; }
        }
    }
}

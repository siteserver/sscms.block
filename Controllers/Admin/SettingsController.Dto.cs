using System.Collections.Generic;
using SSCMS.Block.Models;

namespace SSCMS.Block.Controllers.Admin
{
    public partial class SettingsController
    {
        public class GetRequest
        {
            public int SiteId { get; set; }
        }

        public class GetResult
        {
            public Config Config { get; set; }
            public List<IdName> BlockAreas { get; set; }
            public List<IdName> BlockChannels { get; set; }
        }

        public class SetRequest
        {
            public int SiteId { get; set; }
            public Config Config { get; set; }
            public List<IdName> BlockAreas { get; set; }
            public List<IdName> BlockChannels { get; set; }
        }
    }
}

using System.Collections.Generic;
using SSCMS.Block.Models;
using SSCMS.Dto;

namespace SSCMS.Block.Controllers.Admin
{
    public partial class SettingsLayerAreaAddController
    {
        public class GetResult
        {
            public List<Select<int>> Areas { get; set; }
        }

        public class SubmitRequest : SiteRequest
        {
            public List<int> AreaIds { get; set; }
        }

        public class SubmitResult
        {
            public List<IdName> Areas { get; set; }
        }
    }
}
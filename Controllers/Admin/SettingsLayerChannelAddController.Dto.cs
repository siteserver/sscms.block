using System.Collections.Generic;
using SSCMS.Block.Models;
using SSCMS.Dto;

namespace SSCMS.Block.Controllers.Admin
{
    public partial class SettingsLayerChannelAddController
    {
        public class GetResult
        {
            public Cascade<int> Channels { get; set; }
        }

        public class SubmitRequest : SiteRequest
        {
            public List<int> ChannelIds { get; set; }
        }

        public class SubmitResult
        {
            public List<IdName> Channels { get; set; }
        }
    }
}
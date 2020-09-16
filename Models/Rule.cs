using System.Collections.Generic;
using Datory;
using Datory.Annotations;

namespace SSCMS.Block.Models
{
    [DataTable("sscms_block_rule")]
    public class Rule : Entity
    {
        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
        public string RuleName { get; set; }

        [DataColumn]
        public AreaType AreaType { get; set; }

        [DataColumn]
        public List<int> BlockAreas { get; set; }

        [DataColumn]
        public List<string> AllowList { get; set; }

        [DataColumn]
        public List<string> BlockList { get; set; }

        [DataColumn]
        public bool IsAllChannels { get; set; }

        [DataColumn]
        public List<int> BlockChannels { get; set; }

        [DataColumn]
        public BlockMethod BlockMethod { get; set; }

        [DataColumn]
        public string RedirectUrl { get; set; }

        [DataColumn]
        public string Warning { get; set; }

        [DataColumn]
        public string Password { get; set; }
    }
}
using System.Collections.Generic;

namespace SSCMS.Block.Models
{
    public class Config
    {
        public bool IsEnabled { get; set; }

        public bool IsAllChannels { get; set; }

        public List<int> BlockChannels { get; set; }

        public bool IsAllAreas { get; set; }

        public List<int> BlockAreas { get; set; }

        public string BlockMethod { get; set; }

        public string RedirectUrl { get; set; }

        public string Warning { get; set; }

        public string Password { get; set; }
    }
}
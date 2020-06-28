﻿using System;
using Datory;
using Datory.Annotations;

namespace SSCMS.Block.Models
{
    [DataTable("sscms_block")]
    public class Block : Entity
    {
        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
        public DateTime BlockDate { get; set; }

        [DataColumn]
        public int BlockCount { get; set; }
    }
}

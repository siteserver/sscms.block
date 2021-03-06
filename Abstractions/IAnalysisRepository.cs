﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSCMS.Block.Abstractions
{
    public interface IAnalysisRepository
    {
        Task AddBlockAsync(int siteId);

        Task<List<KeyValuePair<string, int>>> GetMonthlyBlockedListAsync(int siteId);
    }
}

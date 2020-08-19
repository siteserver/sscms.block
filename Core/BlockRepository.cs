using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using SSCMS.Block.Abstractions;
using SSCMS.Services;

namespace SSCMS.Block.Core
{
    public class BlockRepository : IBlockRepository
    {
        private readonly Repository<Models.Block> _repository;

        public BlockRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<Models.Block>(settingsManager.Database);
        }

        private static class Attr
        {
            public const string SiteId = nameof(Models.Block.SiteId);

            public const string BlockDate = nameof(Models.Block.BlockDate);

            public const string BlockCount = nameof(Models.Block.BlockCount);
        }

        public async Task AddBlockAsync(int siteId)
        {
            var now = GetNow();
            var exists = await _repository.ExistsAsync(Q
                .Where(Attr.SiteId, siteId)
                .Where(Attr.BlockDate, now)
            );

            if (exists)
            {
                await _repository.IncrementAsync(Attr.BlockCount, Q
                    .Where(Attr.SiteId, siteId)
                    .Where(Attr.BlockDate, now)
                );
            }
            else
            {
                await _repository.InsertAsync(new Models.Block
                {
                    SiteId = siteId,
                    BlockDate = now,
                    BlockCount = 1
                });
            }
        }

        private static DateTime GetNow()
        {
            var now = DateTime.Now;
            return new DateTime(now.Year, now.Month, now.Day);
        }

        public async Task<List<KeyValuePair<string, int>>> GetMonthlyBlockedListAsync(int siteId)
        {
            var now = GetNow();
            var blockInfoList = await _repository.GetAllAsync(Q
                .Where(Attr.SiteId, siteId)
                .WhereBetween(Attr.BlockDate, now.AddDays(-30), now.AddDays(1))
            );

            var blockedList = new List<KeyValuePair<string, int>>();
            for (var i = 30; i >= 0; i--)
            {
                var date = now.AddDays(-i).ToString("M-d");
                var blockInfo = blockInfoList.FirstOrDefault(x => x.BlockDate.ToString("M-d") == date);
                blockedList.Add(new KeyValuePair<string, int>(date, blockInfo?.BlockCount ?? 0));
            }

            return blockedList;
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using SSCMS.Block.Abstractions;
using SSCMS.Block.Models;
using SSCMS.Services;

namespace SSCMS.Block.Controllers
{
    [Route("api/block")]
    public partial class BlockController : ControllerBase
    {
        private const string Route = "";

        private readonly ISettingsManager _settingsManager;
        private readonly IBlockManager _blockManager;
        private readonly IRuleRepository _ruleRepository;

        public BlockController(ISettingsManager settingsManager, IBlockManager blockManager, IRuleRepository ruleRepository)
        {
            _settingsManager = settingsManager;
            _blockManager = blockManager;
            _ruleRepository = ruleRepository;
        }

        public class QueryRequest
        {
            public int SiteId { get; set; }
            public string SessionId { get; set; }
        }

        public class QueryResult
        {
            public bool IsAllowed { get; set; }
            public BlockMethod BlockMethod { get; set; }
            public string RedirectUrl { get; set; }
            public string Warning { get; set; }
        }

        public class AuthRequest
        {
            public int SiteId { get; set; }
            public string Password { get; set; }
        }

        public class AuthResult
        {
            public bool Success { get; set; }
            public string SessionId { get; set; }
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Block.Abstractions;
using SSCMS.Block.Models;
using SSCMS.Configuration;
using SSCMS.Services;

namespace SSCMS.Block.Controllers.Admin
{
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class QueryController : ControllerBase
    {
        private const string Route = "block/query";

        private readonly IAuthManager _authManager;
        private readonly IBlockManager _blockManager;
        private readonly IRuleRepository _ruleRepository;

        public QueryController(IAuthManager authManager, IBlockManager blockManager, IRuleRepository ruleRepository)
        {
            _authManager = authManager;
            _blockManager = blockManager;
            _ruleRepository = ruleRepository;
        }

        public class GetRequest
        {
            public int SiteId { get; set; }
        }

        public class SubmitRequest
        {
            public int SiteId { get; set; }
            public string IpAddress { get; set; }
        }

        public class SubmitResult
        {
            public bool IsAllowed { get; set; }
            public Area Area { get; set; }
        }

        

        
    }
}

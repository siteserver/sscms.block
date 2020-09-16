using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Block.Abstractions;
using SSCMS.Configuration;
using SSCMS.Services;

namespace SSCMS.Block.Controllers.Admin
{
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class AnalysisController : ControllerBase
    {
        private const string Route = "block/analysis";

        private readonly IAuthManager _authManager;
        private readonly IAnalysisRepository _analysisRepository;

        public AnalysisController(IAuthManager authManager, IAnalysisRepository analysisRepository)
        {
            _authManager = authManager;
            _analysisRepository = analysisRepository;
        }

        public class GetRequest
        {
            public int SiteId { get; set; }
        }

        public class GetResult
        {
            public List<string> Days { get; set; }
            public List<int> Count { get; set; }
        }
    }
}

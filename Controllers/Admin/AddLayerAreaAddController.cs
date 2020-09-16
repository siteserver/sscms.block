using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Block.Abstractions;
using SSCMS.Block.Models;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Block.Controllers.Admin
{
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class AddLayerAreaAddController : ControllerBase
    {
        private const string Route = "block/addLayerAreaAdd";

        private readonly IAuthManager _authManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IBlockManager _blockManager;

        public AddLayerAreaAddController(IAuthManager authManager, ISiteRepository siteRepository, IBlockManager blockManager)
        {
            _authManager = authManager;
            _siteRepository = siteRepository;
            _blockManager = blockManager;
        }

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

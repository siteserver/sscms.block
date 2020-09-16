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
    public partial class AddController : ControllerBase
    {
        private const string Route = "block/add";

        private readonly IAuthManager _authManager;
        private readonly IChannelRepository _channelRepository;
        private readonly IRuleRepository _ruleRepository;
        private readonly IBlockManager _blockManager;

        public AddController(IAuthManager authManager, IChannelRepository channelRepository, IRuleRepository ruleRepository, IBlockManager blockManager)
        {
            _authManager = authManager;
            _channelRepository = channelRepository;
            _ruleRepository = ruleRepository;
            _blockManager = blockManager;
        }

        public class GetRequest
        {
            public int SiteId { get; set; }
            public int RuleId { get; set; }
        }

        public class GetResult
        {
            public Rule Rule { get; set; }
            public List<Select<string>> AreaTypes { get; set; }
            public List<IdName> BlockAreas { get; set; }
            public List<IdName> BlockChannels { get; set; }
        }

        public class SubmitRequest
        {
            public int SiteId { get; set; }
            public Rule Rule { get; set; }
            public List<IdName> BlockAreas { get; set; }
            public List<IdName> BlockChannels { get; set; }
        }
    }
}

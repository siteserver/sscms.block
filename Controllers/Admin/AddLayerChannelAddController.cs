using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Block.Models;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Block.Controllers.Admin
{
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class AddLayerChannelAddController : ControllerBase
    {
        private const string Route = "block/addLayerChannelAdd";

        private readonly IAuthManager _authManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;

        public AddLayerChannelAddController(IAuthManager authManager, ISiteRepository siteRepository, IChannelRepository channelRepository)
        {
            _authManager = authManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
        }

        public class GetResult
        {
            public Cascade<int> Channels { get; set; }
        }

        public class SubmitRequest : SiteRequest
        {
            public List<int> ChannelIds { get; set; }
        }

        public class SubmitResult
        {
            public List<IdName> Channels { get; set; }
        }
    }
}

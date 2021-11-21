using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Block.Abstractions;
using SSCMS.Block.Models;
using SSCMS.Configuration;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Block.Controllers.Admin
{
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class SettingsController : ControllerBase
    {
        private const string Route = "block/settings";
        private const string RouteDelete = "block/settings/actions/delete";

        private readonly IAuthManager _authManager;
        private readonly IChannelRepository _channelRepository;
        private readonly IRuleRepository _ruleRepository;
        public SettingsController(IAuthManager authManager, IChannelRepository channelRepository, IRuleRepository ruleRepository)
        {
            _authManager = authManager;
            _channelRepository = channelRepository;
            _ruleRepository = ruleRepository;
        }

        public class GetResult
        {
            public List<Rule> Rules { get; set; }
        }

        public class DeleteRequest
        {
            public int SiteId { get; set; }
            public int RuleId { get; set; }
        }

        private async Task<string> GetChannelsAsync(int siteId, Rule rule)
        {
            var builder = new StringBuilder();

            if (rule.IsAllChannels)
            {
                builder.Append("所有页面");
            }
            else if (rule.BlockChannels != null)
            {
                foreach (var channelId in rule.BlockChannels)
                {
                    var channelName = await _channelRepository.GetChannelNameNavigationAsync(siteId, channelId);
                    if (!string.IsNullOrEmpty(channelName))
                    {
                        builder.Append(channelName);
                    }
                    builder.Append(",");
                }
                if (builder.Length > 0) builder.Length--;
            }

            return builder.ToString();
        }
    }
}

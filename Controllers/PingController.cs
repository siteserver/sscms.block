using Microsoft.AspNetCore.Mvc;

namespace SSCMS.Block.Controllers
{
    [Route("api/block/ping")]
    public class PingController : ControllerBase
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public string Get()
        {
            return "pong";
        }
    }
}

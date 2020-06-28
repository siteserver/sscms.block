namespace SSCMS.Block.Controllers
{
    public partial class BlockController
    {
        public class QueryRequest
        {
            public int SiteId { get; set; }
            public string SessionId { get; set; }
        }

        public class QueryResult
        {
            public bool IsAllowed { get; set; }
            public string BlockMethod { get; set; }
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

using Microsoft.AspNetCore.Mvc;

namespace SSCMS.Photos.Controllers
{
    [Route("api/photos/ping")]
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
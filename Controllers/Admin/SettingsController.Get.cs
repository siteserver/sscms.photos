using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Photos.Core;

namespace SSCMS.Photos.Controllers.Admin
{
    public partial class SettingsController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, PhotoManager.PermissionsSettings))
                return Unauthorized();

            var settings = await _photoManager.GetSettingsAsync(request.SiteId);

            return new GetResult
            {
                Settings = settings
            };
        }
    }
}

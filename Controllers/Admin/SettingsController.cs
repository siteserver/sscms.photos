using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Photos.Abstractions;
using SSCMS.Photos.Core;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Photos.Controllers.Admin
{
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class SettingsController : ControllerBase
    {
        private const string Route = "photos/settings";

        private readonly IAuthManager _authManager;
        private readonly IPhotoManager _photoManager;

        public SettingsController(IAuthManager authManager, IPhotoManager photoManager)
        {
            _authManager = authManager;
            _photoManager = photoManager;
        }

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

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, PhotoManager.PermissionsSettings))
                return Unauthorized();

            var settings = await _photoManager.GetSettingsAsync(request.SiteId);
            settings.PhotoSmallWidth = request.PhotoSmallWidth;
            settings.PhotoMiddleWidth = request.PhotoMiddleWidth;

            await _photoManager.SetSettingsAsync(request.SiteId, settings);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}

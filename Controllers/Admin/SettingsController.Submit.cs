using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Photos.Core;

namespace SSCMS.Photos.Controllers.Admin
{
    public partial class SettingsController
    {
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

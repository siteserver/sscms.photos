using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Photos.Core;

namespace SSCMS.Photos.Controllers.Admin
{
    public partial class PhotosController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] ContentRequest request)
        {
            if (!await _authManager.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, PhotoManager.PermissionsContent)) return Unauthorized();

            var site = await _siteRepository.GetAsync(request.SiteId);

            var photos = await _photoRepository.GetPhotosAsync(request.SiteId, request.ChannelId, request.ContentId);

            foreach (var photo in photos)
            {
                photo.LargeUrl = await _pathManager.ParseSiteUrlAsync(site, photo.LargeUrl, true);
                photo.MiddleUrl = await _pathManager.ParseSiteUrlAsync(site, photo.MiddleUrl, true);
                photo.SmallUrl = await _pathManager.ParseSiteUrlAsync(site, photo.SmallUrl, true);
            }

            return new GetResult
            {
                Photos = photos
            };
        }
    }
}

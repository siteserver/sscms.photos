using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Photos.Models;

namespace SSCMS.Photos.Controllers.Admin
{
    public partial class PhotosLayerAddController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            if (!await _authManager.IsSiteAdminAsync(request.SiteId)) return Unauthorized();

            var site = await _siteRepository.GetAsync(request.SiteId);
            var siteUrl = await _pathManager.GetSiteUrlAsync(site, true);

            Photo photo = null;
            var largeUrl = string.Empty;
            var middleUrl = string.Empty;
            var smallUrl = string.Empty;
            var description = string.Empty;

            if (request.PhotoId > 0)
            {
                photo = await _photoRepository.GetAsync(request.PhotoId);

                largeUrl = await _pathManager.ParseSiteUrlAsync(site, photo.LargeUrl, true);
                middleUrl = await _pathManager.ParseSiteUrlAsync(site, photo.MiddleUrl, true);
                smallUrl = await _pathManager.ParseSiteUrlAsync(site, photo.SmallUrl, true);
                description = photo.Description;
            }
            
            return new GetResult
            {
                SiteUrl = siteUrl,
                Photo = photo,
                LargeUrl = largeUrl,
                MiddleUrl = middleUrl,
                SmallUrl = smallUrl,
                Description = description
            };
        }
    }
}

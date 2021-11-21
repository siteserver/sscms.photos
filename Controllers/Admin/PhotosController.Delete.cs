using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Photos.Core;

namespace SSCMS.Photos.Controllers.Admin
{
    public partial class PhotosController
    {
        [HttpPost, Route(RouteDelete)]
        public async Task<ActionResult<BoolResult>> Delete([FromBody] DeleteRequest request)
        {
            if (!await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, PhotoManager.PermissionsContent))
                return Unauthorized();

            await _photoManager.DeletePhotoAsync(request.SiteId, request.PhotoId);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}

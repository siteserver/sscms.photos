using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Photos.Core;
using SSCMS.Photos.Models;

namespace SSCMS.Photos.Controllers.Admin
{
    public partial class PhotosLayerAddController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, PhotoManager.PermissionsContent))
                return Unauthorized();

            if (request.PhotoId > 0)
            {
                var photo = await _photoRepository.GetAsync(request.PhotoId);
                photo.LargeUrl = request.LargeUrl;
                photo.MiddleUrl = request.MiddleUrl;
                photo.SmallUrl = request.SmallUrl;
                photo.Description = request.Description;
                await _photoRepository.UpdateAsync(photo);
            }
            else
            {
                var photo = new Photo
                {
                    SiteId = request.SiteId,
                    ChannelId = request.ChannelId,
                    ContentId = request.ContentId,
                    LargeUrl = request.LargeUrl,
                    MiddleUrl = request.MiddleUrl,
                    SmallUrl = request.SmallUrl,
                    Description = request.Description
                };
                await _photoRepository.InsertAsync(photo);
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}

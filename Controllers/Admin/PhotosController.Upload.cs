using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Photos.Core;
using SSCMS.Utils;

namespace SSCMS.Photos.Controllers.Admin
{
    public partial class PhotosController
    {
        [RequestSizeLimit(long.MaxValue)]
        [HttpPost, Route(RouteUpload)]
        public async Task<ActionResult<SubmitResult>> Upload([FromQuery] ContentRequest request, [FromForm] IFormFile file)
        {
            if (!await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, PhotoManager.PermissionsContent))
                return Unauthorized();

            var site = await _siteRepository.GetAsync(request.SiteId);

            if (file == null)
            {
                return this.Error("请选择有效的文件上传");
            }

            var fileName = Path.GetFileName(file.FileName);

            var extName = PathUtils.GetExtension(fileName);
            if (!_pathManager.IsImageExtensionAllowed(site, extName))
            {
                return this.Error(Constants.ErrorImageExtensionAllowed);
            }
            if (!_pathManager.IsImageSizeAllowed(site, file.Length))
            {
                return this.Error(Constants.ErrorImageSizeAllowed);
            }

            var localDirectoryPath = await _pathManager.GetUploadDirectoryPathAsync(site, extName);
            var localFileName = _pathManager.GetUploadFileName(site, fileName);
            var filePath = PathUtils.Combine(localDirectoryPath, localFileName);

            await _pathManager.UploadAsync(file, filePath);
            await _pathManager.AddWaterMarkAsync(site, filePath);

            var photo = await _photoManager.InsertPhotoAsync(site, filePath, request.SiteId, request.ChannelId, request.ContentId);

            photo.LargeUrl = await _pathManager.ParseSiteUrlAsync(site, photo.LargeUrl, true);
            photo.MiddleUrl = await _pathManager.ParseSiteUrlAsync(site, photo.MiddleUrl, true);
            photo.SmallUrl = await _pathManager.ParseSiteUrlAsync(site, photo.SmallUrl, true);

            return new SubmitResult
            {
                Photo = photo
            };
        }
    }
}

using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Extensions;
using SSCMS.Photos.Abstractions;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Photos.Controllers.Admin
{
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class PhotosController : ControllerBase
    {
        private const string Route = "photos/photos";
        private const string RouteUpload = "photos/photos/actions/upload";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IPhotoManager _photoManager;
        private readonly IPhotoRepository _photoRepository;

        public PhotosController(IAuthManager authManager, IPathManager pathManager, ISiteRepository siteRepository, IPhotoManager photoManager, IPhotoRepository photoRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _siteRepository = siteRepository;
            _photoManager = photoManager;
            _photoRepository = photoRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] ContentRequest request)
        {
            if (!await _authManager.IsSiteAdminAsync(request.SiteId)) return Unauthorized();

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

        [HttpPost, Route(RouteUpload)]
        public async Task<ActionResult<SubmitResult>> Upload([FromQuery] ContentRequest request, [FromForm] IFormFile file)
        {
            if (!await _authManager.IsSiteAdminAsync(request.SiteId)) return Unauthorized();

            var site = await _siteRepository.GetAsync(request.SiteId);

            if (file == null)
            {
                return this.Error("请选择有效的文件上传");
            }

            var fileName = Path.GetFileName(file.FileName);

            var extName = PathUtils.GetExtension(fileName);
            if (!_pathManager.IsImageExtensionAllowed(site, extName))
            {
                return this.Error("此图片格式已被禁止上传，请转换格式后上传!");
            }

            var localDirectoryPath = await _pathManager.GetUploadDirectoryPathAsync(site, extName);
            var localFileName = _pathManager.GetUploadFileName(site, fileName);
            var filePath = PathUtils.Combine(localDirectoryPath, localFileName);

            await _pathManager.UploadAsync(file, filePath);

            var photo = await _photoManager.InsertPhotoAsync(site, filePath, request.SiteId, request.ChannelId, request.ContentId);

            photo.LargeUrl = await _pathManager.ParseSiteUrlAsync(site, photo.LargeUrl, true);
            photo.MiddleUrl = await _pathManager.ParseSiteUrlAsync(site, photo.MiddleUrl, true);
            photo.SmallUrl = await _pathManager.ParseSiteUrlAsync(site, photo.SmallUrl, true);

            return new SubmitResult
            {
                Photo = photo
            };
        }

        [HttpPut, Route(Route)]
        public async Task<ActionResult<BoolResult>> Update([FromBody] UpdateRequest request)
        {
            if (!await _authManager.IsSiteAdminAsync(request.SiteId)) return Unauthorized();

            if (request.Type == "description")
            {
                await _photoRepository.UpdateDescriptionAsync(request.PhotoId, request.Description);
            }
            else if (request.Type == "taxis")
            {
                await _photoRepository.UpdateTaxisAsync(request.PhotoIds);
            }

            return new BoolResult
            {
                Value = true
            };
        }

        [HttpDelete, Route(Route)]
        public async Task<ActionResult<BoolResult>> Delete([FromBody] DeleteRequest request)
        {
            if (!await _authManager.IsSiteAdminAsync(request.SiteId)) return Unauthorized();

            await _photoManager.DeletePhotoAsync(request.SiteId, request.PhotoId);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Photos.Abstractions;
using SSCMS.Photos.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Photos.Controllers.Admin
{
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class PhotosLayerAddController : ControllerBase
    {
        private const string Route = "photos/photosLayerAdd";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IPhotoRepository _photoRepository;

        public PhotosLayerAddController(IAuthManager authManager, IPathManager pathManager, ISiteRepository siteRepository, IPhotoRepository photoRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _siteRepository = siteRepository;
            _photoRepository = photoRepository;
        }

        public class GetRequest : ChannelRequest
        {
            public int ContentId { get; set; }
            public int PhotoId { get; set; }
        }

        public class GetResult
        {
            public string SiteUrl { get; set; }
            public Photo Photo { get; set; }
            public string SmallUrl { get; set; }
            public string MiddleUrl { get; set; }
            public string LargeUrl { get; set; }
            public string Description { get; set; }
        }

        public class SubmitRequest : GetRequest
        {
            public string SmallUrl { get; set; }
            public string MiddleUrl { get; set; }
            public string LargeUrl { get; set; }
            public string Description { get; set; }
        }
    }
}

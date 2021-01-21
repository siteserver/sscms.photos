using System.Collections.Generic;
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

        public class ContentRequest : ChannelRequest
        {
            public int ContentId { get; set; }
        }

        public class GetResult
        {
            public List<Photo> Photos { get; set; }
        }

        public class SubmitResult
        {
            public Photo Photo { get; set; }
        }

        public class UpdateRequest : ChannelRequest
        {
            public int PhotoId { get; set; }
            public string Type { get; set; }
            public string Description { get; set; }
            public List<int> PhotoIds { get; set; }
        }

        public class DeleteRequest : ChannelRequest
        {
            public int PhotoId { get; set; }
        }
    }
}

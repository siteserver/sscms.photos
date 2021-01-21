using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Photos.Abstractions;
using SSCMS.Photos.Models;
using SSCMS.Services;

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

        public class GetResult
        {
            public Settings Settings { get; set; }
        }

        public class SubmitRequest : SiteRequest
        {
            public int PhotoSmallWidth { get; set; }

            public int PhotoMiddleWidth { get; set; }
        }
    }
}

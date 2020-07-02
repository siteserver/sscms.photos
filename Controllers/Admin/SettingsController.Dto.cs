using SSCMS.Dto;
using SSCMS.Photos.Models;

namespace SSCMS.Photos.Controllers.Admin
{
    public partial class SettingsController
    {
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

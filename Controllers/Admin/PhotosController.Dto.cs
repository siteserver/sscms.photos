using System.Collections.Generic;
using SSCMS.Dto;
using SSCMS.Photos.Models;

namespace SSCMS.Photos.Controllers.Admin
{
    public partial class PhotosController
    {
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

        public class UpdateRequest : SiteRequest
        {
            public int PhotoId { get; set; }
            public string Type { get; set; }
            public string Description { get; set; }
            public List<int> PhotoIds { get; set; }
        }

        public class DeleteRequest : SiteRequest
        {
            public int PhotoId { get; set; }
        }
    }
}

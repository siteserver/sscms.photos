using Datory;
using Datory.Annotations;

namespace SSCMS.Photos.Models
{
    [DataTable("sscms_photos")]
    public class Photo : Entity
    {
        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
        public int ChannelId { get; set; }

        [DataColumn]
        public int ContentId { get; set; }

        [DataColumn]
        public string SmallUrl { get; set; }

        [DataColumn]
        public string MiddleUrl { get; set; }

        [DataColumn]
        public string LargeUrl { get; set; }

        [DataColumn]
        public int Taxis { get; set; }

        [DataColumn(Length = 2000)]
        public string Description { get; set; }
    }
}

using System.Threading.Tasks;
using SSCMS.Photos.Abstractions;
using SSCMS.Plugins;

namespace SSCMS.Photos.Core
{
    public class ContentHandler : PluginContentHandler
    {
        private readonly IPhotoManager _photoManager;

        public ContentHandler(IPhotoManager photoManager)
        {
            _photoManager = photoManager;
        }

        public override async Task OnDeletedAsync(int siteId, int channelId, int contentId)
        {
            await _photoManager.DeletePhotosAsync(siteId, channelId, contentId);
        }

        public override async Task OnTranslatedAsync(int siteId, int channelId, int contentId, int targetSiteId, int targetChannelId, int targetContentId)
        {
            await _photoManager.TranslatePhotosAsync(siteId, channelId, contentId, targetSiteId, targetChannelId,
                targetContentId);
        }
    }
}

using System.Threading.Tasks;
using SSCMS.Models;
using SSCMS.Photos.Models;

namespace SSCMS.Photos.Abstractions
{
    public interface IPhotoManager
    {
        Task<Settings> GetSettingsAsync(int siteId);

        Task<bool> SetSettingsAsync(int siteId, Settings settings);

        Task<Photo> InsertPhotoAsync(Site site, string filePath, int siteId, int channelId, int contentId);

        Task DeletePhotosAsync(int siteId, int channelId, int contentId);

        Task DeletePhotoAsync(int siteId, int photoId);

        Task TranslatePhotosAsync(int siteId, int channelId, int contentId, int targetSiteId, int targetChannelId, int targetContentId);
    }
}

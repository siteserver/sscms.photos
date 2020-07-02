using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Photos.Models;

namespace SSCMS.Photos.Abstractions
{
    public interface IPhotoRepository
    {
        Task<int> InsertAsync(Photo photoInfo);

        Task UpdateDescriptionAsync(int photoId, string description);

        Task UpdateTaxisAsync(List<int> photoIds);

        Task DeleteAsync(int photoId);

        Task DeleteAsync(int siteId, int channelId, int contentId);

        Task<Photo> GetFirstPhotoAsync(int siteId, int channelId, int contentId);

        Task<int> GetCountAsync(int siteId, int channelId, int contentId);

        Task<List<int>> GetPhotoContentIdListAsync(int siteId, int channelId, int contentId);

        Task<List<Photo>> GetPhotosAsync(int siteId, int channelId, int contentId);

        Task<Photo> GetAsync(int photoId);
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Photos.Abstractions;
using SSCMS.Photos.Models;
using SSCMS.Services;

namespace SSCMS.Photos.Core
{
    public class PhotoRepository : IPhotoRepository
    {
        private readonly Repository<Photo> _repository;

        public PhotoRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<Photo>(settingsManager.Database);
        }

        public async Task<int> InsertAsync(Photo photoInfo)
        {
            var maxTaxis = await GetMaxTaxisAsync(photoInfo.SiteId, photoInfo.ChannelId, photoInfo.ContentId);
            photoInfo.Taxis = maxTaxis + 1;
            photoInfo.Id = await _repository.InsertAsync(photoInfo);

            return photoInfo.Id;
        }

        public async Task UpdateAsync(Photo photo)
        {
            await _repository.UpdateAsync(photo);
        }

        public async Task UpdateDescriptionAsync(int photoId, string description)
        {
            await _repository.UpdateAsync(Q
                .Set(nameof(Photo.Description), description)
                .Where(nameof(Photo.Id), photoId)
            );
        }

        public async Task UpdateTaxisAsync(List<int> photoIds)
        {
            var taxis = 1;
            foreach (var photoId in photoIds)
            {
                await SetTaxisAsync(photoId, taxis);
                taxis++;
            }
        }

        public async Task DeleteAsync(int photoId)
        {
            await _repository.DeleteAsync(photoId);
        }

        public async Task DeleteAsync(int siteId, int channelId, int contentId)
        {
            await _repository.DeleteAsync(Q
                .Where(nameof(Photo.SiteId), siteId)
                .Where(nameof(Photo.ChannelId), channelId)
                .Where(nameof(Photo.ContentId), contentId)
            );
        }

        public async Task<Photo> GetFirstPhotoAsync(int siteId, int channelId, int contentId)
        {
            return await _repository.GetAsync(Q
                .Where(nameof(Photo.SiteId), siteId)
                .Where(nameof(Photo.ChannelId), channelId)
                .Where(nameof(Photo.ContentId), contentId)
                .OrderBy(nameof(Photo.Taxis))
            );
        }

        public async Task<int> GetCountAsync(int siteId, int channelId, int contentId)
        {
            return await _repository.CountAsync(Q
                .Where(nameof(Photo.SiteId), siteId)
                .Where(nameof(Photo.ChannelId), channelId)
                .Where(nameof(Photo.ContentId), contentId)
            );
        }

        public async Task<List<int>> GetPhotoContentIdListAsync(int siteId, int channelId, int contentId)
        {
            return await _repository.GetAllAsync<int>(Q
                .Select(nameof(Photo.Id))
                .Where(nameof(Photo.SiteId), siteId)
                .Where(nameof(Photo.ChannelId), channelId)
                .Where(nameof(Photo.ContentId), contentId)
                .OrderBy(nameof(Photo.Taxis))
            );
        }

        public async Task<List<Photo>> GetPhotosAsync(int siteId, int channelId, int contentId)
        {
            return await _repository.GetAllAsync(Q
                .Where(nameof(Photo.SiteId), siteId)
                .Where(nameof(Photo.ChannelId), channelId)
                .Where(nameof(Photo.ContentId), contentId)
                .OrderBy(nameof(Photo.Taxis))
            );
        }

        public async Task<Photo> GetAsync(int photoId)
        {
            return await _repository.GetAsync(photoId);
        }

        private async Task SetTaxisAsync(int id, int taxis)
        {
            await _repository.UpdateAsync(Q
                .Set(nameof(Photo.Taxis), taxis)
                .Where(nameof(Photo.Id), id)
            );
        }

        private async Task<int> GetMaxTaxisAsync(int siteId, int channelId, int contentId)
        {
            return await _repository.MaxAsync(nameof(Photo.Taxis), Q
                       .Where(nameof(Photo.SiteId), siteId)
                       .Where(nameof(Photo.ChannelId), channelId)
                       .Where(nameof(Photo.ContentId), contentId)
                   ) ?? 0;
        }
    }
}
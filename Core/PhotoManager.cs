using System.Threading.Tasks;
using SSCMS.Models;
using SSCMS.Photos.Abstractions;
using SSCMS.Photos.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Photos.Core
{
    public class PhotoManager : IPhotoManager
    {
        public const string PluginId = "sscms.photos";
        public const string PermissionsSettings = "photos_settings";

        private readonly IPathManager _pathManager;
        private readonly IPluginConfigRepository _pluginConfigRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IPhotoRepository _photoRepository;

        public PhotoManager(IPathManager pathManager, IPluginConfigRepository pluginConfigRepository, ISiteRepository siteRepository, IPhotoRepository photoRepository)
        {
            _pathManager = pathManager;
            _pluginConfigRepository = pluginConfigRepository;
            _siteRepository = siteRepository;
            _photoRepository = photoRepository;
        }

        public async Task<Settings> GetSettingsAsync(int siteId)
        {
            return await _pluginConfigRepository.GetConfigAsync<Settings>(PluginId, siteId) ?? new Settings();
        }

        public async Task<bool> SetSettingsAsync(int siteId, Settings settings)
        {
            return await _pluginConfigRepository.SetConfigAsync(PluginId, siteId, settings);
        }

        public async Task DeletePhotosAsync(int siteId, int channelId, int contentId)
        {
            var site = await _siteRepository.GetAsync(siteId);
            var photos = await _photoRepository.GetPhotosAsync(siteId, channelId, contentId);
            foreach (var photo in photos)
            {
                var filePath = await _pathManager.ParseSitePathAsync(site, photo.SmallUrl);
                FileUtils.DeleteFileIfExists(filePath);
                filePath = await _pathManager.ParseSitePathAsync(site, photo.MiddleUrl);
                FileUtils.DeleteFileIfExists(filePath);
                filePath = await _pathManager.ParseSitePathAsync(site, photo.LargeUrl);
                FileUtils.DeleteFileIfExists(filePath);
            }
            await _photoRepository.DeleteAsync(siteId, channelId, contentId);
        }

        public async Task DeletePhotoAsync(int siteId, int photoId)
        {
            var site = await _siteRepository.GetAsync(siteId);
            var photo = await _photoRepository.GetAsync(photoId);
            var filePath = await _pathManager.ParseSitePathAsync(site, photo.SmallUrl);
            FileUtils.DeleteFileIfExists(filePath);
            filePath = await _pathManager.ParseSitePathAsync(site, photo.MiddleUrl);
            FileUtils.DeleteFileIfExists(filePath);
            filePath = await _pathManager.ParseSitePathAsync(site, photo.LargeUrl);
            FileUtils.DeleteFileIfExists(filePath);
            await _photoRepository.DeleteAsync(photoId);
        }

        public async Task TranslatePhotosAsync(int siteId, int channelId, int contentId, int targetSiteId, int targetChannelId, int targetContentId)
        {
            var photos = await _photoRepository.GetPhotosAsync(siteId, channelId, contentId);
            if (photos == null || photos.Count == 0) return;

            var sourceSite = await _siteRepository.GetAsync(siteId);
            var targetSite = await _siteRepository.GetAsync(targetSiteId);

            foreach (var photo in photos)
            {
                photo.SiteId = targetSiteId;
                photo.ChannelId = targetChannelId;
                photo.ContentId = targetContentId;

                if (siteId != targetSiteId)
                {
                    await _pathManager.MoveFileAsync(sourceSite, targetSite, photo.SmallUrl);
                    await _pathManager.MoveFileAsync(sourceSite, targetSite, photo.MiddleUrl);
                    await _pathManager.MoveFileAsync(sourceSite, targetSite, photo.LargeUrl);
                }

                await _photoRepository.InsertAsync(photo);
            }
        }

        public async Task<Photo> InsertPhotoAsync(Site site, string filePath, int siteId, int channelId, int contentId)
        {
            var settings = await GetSettingsAsync(siteId);

            var largeUrl = await _pathManager.GetVirtualUrlByPhysicalPathAsync(site, filePath);

            var smallUrl = largeUrl;
            var middleUrl = largeUrl;

            var (width, height) = _pathManager.GetImageSize(filePath);

            if (width > settings.PhotoSmallWidth)
            {
                smallUrl = await ResizeAsync(site, filePath, width, height, settings.PhotoSmallWidth);
            }
            if (width > settings.PhotoMiddleWidth)
            {
                middleUrl = await ResizeAsync(site, filePath, width, height, settings.PhotoMiddleWidth);
            }

            var photoInfo = new Photo
            {
                SiteId = siteId,
                ChannelId = channelId,
                ContentId = contentId,
                SmallUrl = smallUrl,
                MiddleUrl = middleUrl,
                LargeUrl = largeUrl
            };
            photoInfo.Id = await _photoRepository.InsertAsync(photoInfo);
            return photoInfo;
        }

        private async Task<string> ResizeAsync(Site site, string filePath, int width, int height, int maxWidth)
        {
            var ratio = height / (double)width;
            height = (int)(maxWidth * ratio);

            var resizeFileName = $"{PathUtils.GetFileNameWithoutExtension(filePath)}_{maxWidth}{PathUtils.GetExtension(filePath)}";
            var resizeFilePath = PathUtils.Combine(DirectoryUtils.GetDirectoryPath(filePath), resizeFileName);

            _pathManager.ResizeImage(filePath, resizeFilePath, maxWidth, height);

            return await _pathManager.GetVirtualUrlByPhysicalPathAsync(site, resizeFilePath);
        }
    }
}

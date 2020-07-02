using System.Collections.Generic;
using SiteServer.Plugin;
using SSCMS.Photos.Core;
using Menu = SiteServer.Plugin.Menu;

namespace SSCMS.Photos
{
    public class Main : PluginBase
    {
        private static readonly Dictionary<int, ConfigInfo> ParmConfigInfoDict = new Dictionary<int, ConfigInfo>();

        public static ConfigInfo GetConfigInfo(int siteId)
        {
            if (!ParmConfigInfoDict.ContainsKey(siteId))
            {
                ParmConfigInfoDict[siteId] = Context.ConfigApi.GetConfig<ConfigInfo>(Utils.PluginId, siteId) ?? new ConfigInfo();
            }
            return ParmConfigInfoDict[siteId];
        }

        public override void Startup(IService service)
        {
            var repository = new PhotoRepository();

            service
                .AddSiteMenu(siteId => new Menu
                {
                    Text = "内容相册",
                    IconClass = "ion-images",
                    Menus = new List<Menu>
                    {
                        new Menu
                        {
                            Text = "图片上传设置",
                            Href = "pages/settings.html"
                        }
                    }
                })
                .AddContentMenu(contentInfo => new Menu
                {
                    Text = "内容相册",
                    Href = "pages/photos.html"
                })
                .AddDatabaseTable(repository.TableName, repository.TableColumns)
                .AddStlElementParser(StlPhotos.ElementName, StlPhotos.Parse)
                .AddStlElementParser(StlPhoto.ElementName, StlPhoto.Parse)
                .AddStlElementParser(StlSlide.ElementName, StlSlide.Parse)
                ;

            service.ContentTranslateCompleted += Service_ContentTranslateCompleted;
            service.ContentDeleteCompleted += Service_ContentDeleteCompleted;
        }

        private static void Service_ContentDeleteCompleted(object sender, ContentEventArgs e)
        {
            var repository = new PhotoRepository();
            repository.Delete(e.SiteId, e.ChannelId, e.ContentId);
        }

        private void Service_ContentTranslateCompleted(object sender, ContentTranslateEventArgs e)
        {
            var repository = new PhotoRepository();

            var photoInfoList = repository.GetPhotoInfoList(e.SiteId, e.ChannelId, e.ContentId);
            if (photoInfoList.Count <= 0) return;

            foreach (var photoInfo in photoInfoList)
            {
                photoInfo.SiteId = e.TargetSiteId;
                photoInfo.ChannelId = e.TargetChannelId;
                photoInfo.ContentId = e.TargetContentId;

                if (e.SiteId != e.TargetSiteId)
                {
                    Context.SiteApi.MoveFiles(e.SiteId, e.TargetSiteId, new List<string>
                    {
                        photoInfo.SmallUrl,
                        photoInfo.MiddleUrl,
                        photoInfo.LargeUrl
                    });
                }

                repository.Insert(photoInfo);
            }
        }
    }
}
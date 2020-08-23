using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SSCMS.Parse;
using SSCMS.Photos.Abstractions;
using SSCMS.Photos.Models;
using SSCMS.Plugins;
using SSCMS.Utils;

namespace SSCMS.Photos.Core
{
    public class StlPhotos : IPluginParseAsync
    {
        private const string AttributeTotalNum = "totalNum";
        private const string AttributeStartNum = "startNum";

        private readonly IPhotoRepository _photoRepository;

        public StlPhotos(IPhotoRepository photoRepository)
        {
            _photoRepository = photoRepository;
        }

        public string ElementName => "stl:photos";

        public async Task<string> ParseAsync(IParseStlContext context)
        {
            var photos = await _photoRepository.GetPhotosAsync(context.SiteId, context.ChannelId, context.ContentId);
            if (photos == null || photos.Count == 0) return string.Empty;

            var totalNum = 0;
            var startNum = 1;

            foreach (var name in context.StlAttributes.AllKeys)
            {
                var value = context.StlAttributes[name];

                if (StringUtils.EqualsIgnoreCase(name, AttributeTotalNum))
                {
                    totalNum = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeStartNum))
                {
                    startNum = TranslateUtils.ToInt(value);
                }
            }

            if (startNum > 1 || totalNum > 0)
            {
                photos = photos.Skip(startNum - 1).Take(totalNum).ToList();
            }

            return await ParseImplAsync(photos, context);
        }

        private async Task<string> ParseImplAsync(IEnumerable<Photo> photos, IParseStlContext context)
        {
            var parsedContent = string.Empty;

            var itemIndex = 1;
            foreach (var photo in photos)
            {
                //StlPhoto.SetContextItem(context, photo, itemIndex++);
                //parsedContent += Context.ParseApi.Parse(context.StlInnerHtml, context);

                StlPhoto.SetContextItem(context, photo, itemIndex++);
                //var builder = new StringBuilder(context.StlInnerHtml);
                //await _parseManager.ParseInnerContentAsync(builder);
                //parsedContent += builder;

                parsedContent += await context.ParseAsync(context.StlInnerHtml);
            }

            return parsedContent;
        }
    }
}

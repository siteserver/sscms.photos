using System.Collections.Specialized;
using System.Threading.Tasks;
using SSCMS.Parse;
using SSCMS.Photos.Models;
using SSCMS.Plugins;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Photos.Core
{
    public class StlPhoto : IPluginParseAsync
    {
        private readonly ISiteRepository _siteRepository;
        private readonly IPathManager _pathManager;

        public StlPhoto(ISiteRepository siteRepository, IPathManager pathManager)
        {
            _siteRepository = siteRepository;
            _pathManager = pathManager;
        }


        private const string AttributeType = "type";
        private const string AttributeLeftText = "leftText";
        private const string AttributeRightText = "rightText";
        private const string AttributeFormatString = "formatString";
        private const string AttributeStartIndex = "startIndex";
        private const string AttributeLength = "length";
        private const string AttributeWordNum = "wordNum";
        private const string AttributeEllipsis = "ellipsis";
        private const string AttributeReplace = "replace";
        private const string AttributeTo = "to";
        private const string AttributeIsClearTags = "isClearTags";
        private const string AttributeIsClearBlank = "isClearBlank";
        private const string AttributeIsReturnToBr = "isReturnToBr";
        private const string AttributeIsLower = "isLower";
        private const string AttributeIsUpper = "isUpper";

        private const string TypeItemIndex = "ItemIndex";
        private const string TypeId = "Id";
        private const string TypeSmallUrl = "SmallUrl";
        private const string TypeMiddleUrl = "MiddleUrl";
        private const string TypeLargeUrl = "LargeUrl";
        private const string TypeDescription = "Description";

        public string ElementName => "stl:photo";

        public async Task<string> ParseAsync(IParseStlContext context)
        {
            var leftText = string.Empty;
            var rightText = string.Empty;
            var formatString = string.Empty;
            var startIndex = 0;
            var length = 0;
            var wordNum = 0;
            var ellipsis = "...";
            var replace = string.Empty;
            var to = string.Empty;
            var isClearTags = false;
            var isClearBlank = false;
            var isReturnToBr = false;
            var isLower = false;
            var isUpper = false;
            var type = string.Empty;

            foreach (var name in context.StlAttributes.AllKeys)
            {
                var attributeName = name.ToLower();
                var value = context.StlAttributes[name];

                if (attributeName.Equals(AttributeType))
                {
                    type = value.ToLower();
                }
                else if (attributeName.Equals(AttributeLeftText))
                {
                    leftText = value;
                }
                else if (attributeName.Equals(AttributeRightText))
                {
                    rightText = value;
                }
                else if (attributeName.Equals(AttributeFormatString))
                {
                    formatString = value;
                }
                else if (attributeName.Equals(AttributeStartIndex))
                {
                    startIndex = TranslateUtils.ToInt(value);
                }
                else if (attributeName.Equals(AttributeLength))
                {
                    length = TranslateUtils.ToInt(value);
                }
                else if (attributeName.Equals(AttributeWordNum))
                {
                    wordNum = TranslateUtils.ToInt(value);
                }
                else if (attributeName.Equals(AttributeEllipsis))
                {
                    ellipsis = value;
                }
                else if (attributeName.Equals(AttributeReplace))
                {
                    replace = value;
                }
                else if (attributeName.Equals(AttributeTo))
                {
                    to = value;
                }
                else if (attributeName.Equals(AttributeIsClearTags))
                {
                    isClearTags = TranslateUtils.ToBool(value, false);
                }
                else if (attributeName.Equals(AttributeIsClearBlank))
                {
                    isClearBlank = TranslateUtils.ToBool(value, false);
                }
                else if (attributeName.Equals(AttributeIsReturnToBr))
                {
                    isReturnToBr = TranslateUtils.ToBool(value, false);
                }
                else if (attributeName.Equals(AttributeIsLower))
                {
                    isLower = TranslateUtils.ToBool(value, true);
                }
                else if (attributeName.Equals(AttributeIsUpper))
                {
                    isUpper = TranslateUtils.ToBool(value, true);
                }
            }

            return !TryGetContextItem(context, out var photo, out var itemIndex)
                ? string.Empty
                : await ParseImplAsync(context, photo, itemIndex, leftText, rightText, formatString, startIndex, length, wordNum,
                    ellipsis, replace, to, isClearTags, isClearBlank, isReturnToBr, isLower, isUpper, type);
        }

        public static void SetContextItem(IParseContext context, Photo photo, int itemIndex)
        {
            context.Set($"{nameof(StlPhoto)}:photo", photo);
            context.Set($"{nameof(StlPhoto)}:itemIndex", itemIndex);
        }

        private static bool TryGetContextItem(IParseContext context, out Photo photo, out int itemIndex)
        {
            photo = context.Get<Photo>($"{nameof(StlPhoto)}:photo");
            itemIndex = context.Get<int>($"{nameof(StlPhoto)}:itemIndex");

            return photo != null && itemIndex > 0;
        }

        private async Task<string> ParseImplAsync(IParseStlContext context, Photo photo, int itemIndex, string leftText, string rightText, string formatString, int startIndex, int length, int wordNum, string ellipsis, string replace, string to, bool isClearTags, bool isClearBlank, bool isReturnToBr, bool isLower, bool isUpper, string type)
        {
            var parsedContent = string.Empty;

            if (!string.IsNullOrEmpty(type))
            {
                if (!string.IsNullOrEmpty(formatString))
                {
                    formatString = formatString.Trim();
                    if (!formatString.StartsWith("{0"))
                    {
                        formatString = "{0:" + formatString;
                    }
                    if (!formatString.EndsWith("}"))
                    {
                        formatString = formatString + "}";
                    }
                }
                else
                {
                    formatString = "{0}";
                }

                if (string.IsNullOrEmpty(type) || StringUtils.EqualsIgnoreCase(type, "imageUrl"))
                {
                    type = TypeLargeUrl;
                }

                if (StringUtils.StartsWithIgnoreCase(type, TypeItemIndex))
                {
                    parsedContent = !string.IsNullOrEmpty(formatString) ? string.Format(formatString, itemIndex) : itemIndex.ToString();
                }
                else if (StringUtils.StartsWithIgnoreCase(type, TypeId))
                {
                    parsedContent = !string.IsNullOrEmpty(formatString) ? string.Format(formatString, photo.Id) : photo.Id.ToString();
                }
                else if (StringUtils.StartsWithIgnoreCase(type, TypeSmallUrl))
                {
                    var site = await _siteRepository.GetAsync(context.SiteId);
                    var imageUrl = await _pathManager.ParseSiteUrlAsync(site, photo.SmallUrl, false);
                    parsedContent = GetImageHtml(imageUrl, context.StlAttributes, context.IsStlEntity);
                }
                else if (StringUtils.StartsWithIgnoreCase(type, TypeMiddleUrl))
                {
                    var site = await _siteRepository.GetAsync(context.SiteId);
                    var imageUrl = await _pathManager.ParseSiteUrlAsync(site, photo.MiddleUrl, false);
                    parsedContent = GetImageHtml(imageUrl, context.StlAttributes, context.IsStlEntity);
                }
                else if (StringUtils.StartsWithIgnoreCase(type, TypeLargeUrl))
                {
                    var site = await _siteRepository.GetAsync(context.SiteId);
                    var imageUrl = await _pathManager.ParseSiteUrlAsync(site, photo.LargeUrl, false);
                    parsedContent = GetImageHtml(imageUrl, context.StlAttributes, context.IsStlEntity);
                }
                else if (StringUtils.StartsWithIgnoreCase(type, TypeDescription) || StringUtils.StartsWithIgnoreCase(type, "content"))
                {
                    parsedContent = StringUtils.ReplaceNewlineToBr(photo.Description);
                }
            }

            if (!string.IsNullOrEmpty(parsedContent))
            {
                parsedContent = StringUtils.ParseString(parsedContent, replace, to, startIndex, length, wordNum, ellipsis, isClearTags, isClearBlank, isReturnToBr, isLower, isUpper, formatString);

                if (!string.IsNullOrEmpty(parsedContent))
                {
                    parsedContent = leftText + parsedContent + rightText;
                }
            }

            return parsedContent;
        }

        private static string GetImageHtml(string imageUrl, NameValueCollection attributes, bool isStlEntity)
        {
            if (string.IsNullOrEmpty(imageUrl)) return string.Empty;

            string retVal;

            if (isStlEntity)
            {
                retVal = imageUrl;
            }
            else
            {
                attributes["src"] = imageUrl;
                return $"<img {TranslateUtils.ToAttributesString(attributes)} />";
            }
            return retVal;
        }
    }
}

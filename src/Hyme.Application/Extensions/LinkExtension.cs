using Hyme.Application.Common;

namespace Hyme.Application.Extensions
{

    /// <summary>
    /// 
    /// </summary>
    public static class LinkExtension
    {

        /// <summary>
        /// File name to storage link
        /// </summary>
        /// <param name="fileName">File Name</param>
        /// <param name="type">Link Type </param>
        /// <returns></returns>
        public static string ToLink(this string fileName, LinkType type)
        {
            if (string.IsNullOrEmpty(fileName))
                return string.Empty;

            return type switch
            {
                LinkType.Image => $"https://hymestorage.blob.core.windows.net/image-original/{fileName}",
                LinkType.Video => $"https://hymestorage.blob.core.windows.net/video-original/{fileName}",
                _ => ""
            };
        }
    }
}

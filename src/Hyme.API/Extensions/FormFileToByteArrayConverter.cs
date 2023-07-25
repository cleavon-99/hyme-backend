namespace Hyme.API.Extensions
{

    /// <summary>
    /// Converts IFormFile file to byte[]
    /// </summary>
    public static class FormFileToByteArrayConverter
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file">IFormFile</param>
        /// <returns></returns>
        public static async Task<byte[]> ToByteArrayAsync(this IFormFile file)
        {
            using MemoryStream stream = new();
            await file.CopyToAsync(stream);
            return stream.ToArray();
        }
    }
}

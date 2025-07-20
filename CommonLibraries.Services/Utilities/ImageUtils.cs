using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.Utilities
{
    public class ImageUtils
    {
        public async static Task<byte[]> DownloadImageAsync(string url, string defaultImageUrl)
        {
            try
            {
                using var client = new HttpClient();
                return await client.GetByteArrayAsync(url);
            }
            catch {
                using var client = new HttpClient();
                return await client.GetByteArrayAsync(defaultImageUrl);
            }
        }
    }
}

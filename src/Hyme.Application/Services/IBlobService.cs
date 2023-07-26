using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyme.Application.Services
{
    public interface IBlobService
    {
        Task UploadImageAsync(byte[] image, string fileName);
        Task UploadVideoAsync(byte[] video, string fileName);
        Task DeleteImageAsync(string fileName);
        Task DeleteVideoAsync(string fileName);
    }
}

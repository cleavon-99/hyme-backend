using Microsoft.AspNetCore.Http;

namespace Hyme.Application.DTOs.Request
{
    public class NFTRequest
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public IFormFile Image { get; set; } = null!;
    }
}

namespace Hyme.Application.DTOs.Response
{
    public class NFTResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Image { get; set; } = null!;
        public DateTime DateCreated { get; set; }
    }
}

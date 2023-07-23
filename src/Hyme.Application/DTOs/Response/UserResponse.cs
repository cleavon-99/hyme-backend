namespace Hyme.Application.DTOs.Response
{
    public class UserResponse
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string WalletAddress { get; set; } = null!;
    }
}

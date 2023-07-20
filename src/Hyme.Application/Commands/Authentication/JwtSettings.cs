namespace Hyme.Application.Commands.Authentication
{
    public class JwtSettings
    {

        public const string SectionName = "JwtSettings";
        public string Secret { get; set; } = null!;
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public string TokenLifeSpan { get; set; } = null!;
    }
}

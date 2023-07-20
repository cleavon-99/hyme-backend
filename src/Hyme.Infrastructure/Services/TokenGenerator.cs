using Hyme.Application.Common;
using Hyme.Application.Services;
using Hyme.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Hyme.Infrastructure.Services
{
    public class TokenGenerator : ITokenGenerator
    {
        private readonly JwtSettings _jwtSettings;

        public TokenGenerator(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public string GenerateToken(User userProfile)
        {
            List<Claim> claims = new()
            {
                new Claim(JwtRegisteredClaimNames.Sub, userProfile.Id.Value.ToString()),
            };

            foreach (var item in userProfile.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, item.Name));
            }

            SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            JwtSecurityToken token = new(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.Parse(_jwtSettings.TokenLifeSpan)),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
            
            string tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenAsString;
        }
    }
}

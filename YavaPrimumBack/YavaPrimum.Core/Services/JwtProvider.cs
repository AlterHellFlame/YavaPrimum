using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using YavaPrimum.Core.DataBase.Models;
using YavaPrimum.Core.Interfaces;

namespace YavaPrimum.Core.Services
{
    public class JwtProvider : IJwtProvider
    {
        public static string JwtKey = "secretkeysecretkeysecretkeysecretkeysecretkeysecretkey";//6
        public static string CookiesName = "token-cookies";

        public string GenerateToken(User user)
        {
            Claim[] claims = [new("userId", user.UserId.ToString())];

            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey)),
                    SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                signingCredentials: signingCredentials,
                expires: DateTime.UtcNow.AddHours(12));

            var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenValue;
        }

        public async Task<Guid> GetUserIdFromToken(string token)
        {
            var jsonToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;
            return Guid.Parse(jsonToken.Claims.First(claim => claim.Type == "userId").Value);
        }
    }
}

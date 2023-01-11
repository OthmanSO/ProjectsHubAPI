using Microsoft.IdentityModel.Tokens;
using ProjectsHub.Core;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ProjectsHub.Exceptions;
using System.Text;

namespace ProjectsHub.API.Controllers
{
    public class UserToken : IUserToken
    {
        private readonly IConfiguration _Configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserToken(IConfiguration _conf, IHttpContextAccessor httpContextAccessor)
        {
            _Configuration = _conf ?? throw new ArgumentNullException(nameof(IConfiguration));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(HttpContextAccessor));
        }
        public string CreateUserToken(Guid userId, string UserName, string Email)
        {
            var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier , userId.ToString() ),
                    new Claim(ClaimTypes.GivenName , UserName),
                    new Claim(ClaimTypes.Email, Email)
                };

            var tok = new JwtSecurityToken(
                issuer: _Configuration["Jwt:Issuer"],
                audience: _Configuration["Jwt: Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                notBefore: DateTime.UtcNow,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_Configuration["Jwt:Key"])),
                    SecurityAlgorithms.HmacSha256)
                );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(tok);
            return tokenString;
        }


        public Guid GetUserIdFromToken()
        {
            Guid userId = Guid.Parse(_httpContextAccessor.HttpContext.User.Claims.First(i => i.Type == ClaimTypes.NameIdentifier).Value);
            if (userId == Guid.Empty)
            {
                throw new UserNotLoggedInException();
            }
            return userId;
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProjectsHub.API.Services;
using ProjectsHub.Data;
using ProjectsHub.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProjectHub.API.Controllers
{
    [ApiController]
    [Route("/api/V1.0/usres")]
    public class UserController : ControllerBase
    {
        private readonly UserService _UserService;
        private readonly UserRepository _UserRepository;
        IConfiguration _Configuration;
        public UserController(UserService userService, UserRepository userRepository, IConfiguration _conf)
        {
            _UserService = userService ?? throw new ArgumentNullException(nameof(UserService));
            _UserRepository = userRepository ?? throw new ArgumentNullException(nameof(UserRepository));
            _UserRepository.CreateList();
            _Configuration = _conf ?? throw new ArgumentNullException(nameof(IConfiguration));
        }

        [HttpPost("/login")]
        public async Task<ActionResult> Login([FromBody] UserAuth user)
        {
            if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
                return BadRequest();

            var loggedInUser = _UserService.GetUser(user.Email, user.Password, _UserRepository);
            if (loggedInUser == null)
                return NotFound();
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier , loggedInUser.UserName ),
                new Claim(ClaimTypes.Email, loggedInUser.Email),

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
            return Ok(tokenString);
        }

    }
}

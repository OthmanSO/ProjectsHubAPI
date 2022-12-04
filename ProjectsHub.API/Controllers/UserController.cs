using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProjectsHub.API.Services;
using ProjectsHub.Data;
using ProjectsHub.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ProjectsHub.API.Exceptions;
using Microsoft.AspNetCore.Authorization;
using ProjectsHub.API.Model;

namespace ProjectsHub.API.Controllers
{
    [ApiController]
    [Route("/api/V1.0/user")]
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

        [HttpPost("signup")]
        public async Task<ActionResult> SignUp([FromBody] UserAccountCreate user)
        {
            if (user == null
                || string.IsNullOrEmpty(user.FirstName)
                || string.IsNullOrEmpty(user.LastName)
                || string.IsNullOrEmpty(user.Email)
                || string.IsNullOrEmpty(user.Password))
                return BadRequest("Required feild missing");
            try
            {
                var userId = _UserService.CreateUser(user, _UserRepository);

                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier , userId.ToString() ),
                    new Claim(ClaimTypes.GivenName , $"{user.FirstName} {user.LastName}"),
                    new Claim(ClaimTypes.Email, user.Email)
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
                return Created(userId.ToString(), tokenString);

            }
            catch (UserAlreadyExistException ex)
            {
                return Conflict("User Already Exists");
            }

            return NotFound();

        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] UserAuth user)
        {
            if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
                return BadRequest("Missing Email or Password");
            try
            {
                var loggedInUser = _UserService.GetLoggedInUser(user.Email, user.Password, _UserRepository);
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier , loggedInUser._Id.ToString() ),
                    new Claim(ClaimTypes.GivenName , $"{loggedInUser.FirstName} {loggedInUser.LastName}"),
                    new Claim(ClaimTypes.Email, loggedInUser.Email)
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
            catch (UserPasswordNotMatchedException e)
            {
                return BadRequest("Password Mismatch");
            }
            catch (Exception e)
            {
                return NotFound("User Not Found");
            }
        }

        [HttpPut("ProfilePicture/{id}")]
        public async Task<IActionResult> ChangeProfilePic([FromBody] UserprofilePictureDto ProfilePic, string id)
        {
            if (ProfilePic.EncodedProfilePicture.IsNullOrEmpty() || id.IsNullOrEmpty())
            {
                return BadRequest();
            }
            try
            {
                _UserService.ChangeProfilePic(Guid.Parse(id), ProfilePic.EncodedProfilePicture, _UserRepository);
                return Ok();
            }
            catch (Exception e)
            {
                return NotFound("User not found");
            }
        }
        [HttpPut("Bio/{id}")]
        public async Task<IActionResult> ChangeBio([FromBody] BioDto UserBio, string id)
        {
            if (UserBio.bio.IsNullOrEmpty() || id.IsNullOrEmpty())
            {
                return BadRequest();
            }
            try
            {
                _UserService.ChangeUserBio(Guid.Parse(id), UserBio.bio, _UserRepository);
                return Ok();
            }
            catch (Exception e)
            {
                return NotFound("User not found");
            }
        }

        [HttpPut("Password/{id}")]
        public async Task<IActionResult> ChangePassword([FromBody] PasswordUpdateDto userPasswords, string id)
        {
            if (userPasswords.OldPassword.IsNullOrEmpty() || id.IsNullOrEmpty() || userPasswords.NewPassword.IsNullOrEmpty())
            {
                return BadRequest();
            }
            try
            {
                _UserService.ChangeUserPassword(Guid.Parse(id), userPasswords, _UserRepository);
                return Ok();
            }
            catch (UserPasswordNotMatchedException e)
            {
                return Unauthorized("Old Password Mismatch");
            }
            catch (ArgumentNullException e)
            {
                return NotFound("User Not Found");
            }
        }

        [HttpPut("username/{id}")]
        public async Task<IActionResult> ChangeUsername([FromBody] UserNameDto UserName, string id)
        {
            if (UserName.FirstName.IsNullOrEmpty() || UserName.LastName.IsNullOrEmpty() || id.IsNullOrEmpty())
            {
                return BadRequest();
            }
            try
            {
                _UserService.ChangeUserName(Guid.Parse(id), UserName, _UserRepository);
                return Ok();
            }
            catch (Exception e)
            {
                return NotFound("User not found");
            }
        }

        [HttpPut("Contacts/{id}")]
        public async Task<IActionResult> AddContacts([FromBody] ContactDto Contact, string id)
        {
            if (Contact.ContactId.IsNullOrEmpty() || id.IsNullOrEmpty())
            {
                return BadRequest();
            }
            try
            {
                _UserService.AddContact(Guid.Parse(id), Guid.Parse(Contact.ContactId), _UserRepository);
                return Ok();
            }
            catch (FormatException e)
            {
                return BadRequest();
            }
            catch (ArgumentNullException e)
            {
                return NotFound("User not found");
            }
            catch (InvalidOperationException e)
            {
                return NotFound("user Not Found");
            }
        }

        [HttpDelete("Contacts/{id}")]
        public async Task<IActionResult> DeleteContacts([FromBody] ContactDto Contact, string id)
        {
            if (Contact.ContactId.IsNullOrEmpty() || id.IsNullOrEmpty())
            {
                return BadRequest();
            }
            try
            {
                _UserService.DeleteContact(Guid.Parse(id), Guid.Parse(Contact.ContactId), _UserRepository);
            }
            catch (FormatException e)
            {
                return BadRequest();
            }
            catch (Exception e)
            {
                return Ok();
            }
            return Ok();
        }

        [HttpGet("Contacts/{id}")]
        public async Task<IActionResult> UserContacts(string id)
        {
            var userId = new Guid();

            if (id == null)
            {
                userId = getUserIdFromToken();
            }
            else
            {
                userId = Guid.Parse(id);
            }

            if (userId == Guid.Empty)
            {
                return BadRequest("Log in or include user identifier first");
            }
            try
            {
                var Contacts = _UserService.GetUserContacts(userId, _UserRepository);
                List<IdDto> ContactsList = new List<IdDto>();
                foreach (var Contact in Contacts)
                {
                    ContactsList.Add(new IdDto { Id = Contact });
                }
                return Ok(ContactsList);
            }
            catch (ArgumentNullException e)
            {
                return NotFound("user Not Found");
            }
            catch (InvalidOperationException e)
            {
                return NotFound("user Not Found");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> userProfile(string id)
        {
            var userId = new Guid();

            if (id == null)
            {
                userId = getUserIdFromToken();
            }

            else
            {
                userId = Guid.Parse(id);
            }

            if (userId == Guid.Empty)
            {
                return BadRequest("Log in or include user identifier first");
            }

            var userProfile = _UserService.GetUserProfileById(userId, _UserRepository);

            if (userProfile == null)
                return NotFound("user Not Found");
            return Ok(userProfile);
        }

        [HttpGet("shortProfile/{id}")]
        public async Task<IActionResult> UserShortProfile(string id)
        {
            var userId = new Guid();

            try
            {
                var userShortProfile =  _UserService.GetUserShortPeofile(Guid.Parse(id), _UserRepository);
                return Ok(userShortProfile);
            }
            catch (FormatException e)
            {
                return BadRequest("Wrong userId");
            }
            catch (ArgumentNullException e)
            {
                return NotFound("user Not Found");
            }
            catch (InvalidOperationException e)
            {
                return NotFound("user Not Found");
            }
        }

        private Guid getUserIdFromToken()
        {
            Guid userId;
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var userClaims = identity.Claims;
            userId = Guid.Parse(userClaims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            return userId;
        }
    }
}

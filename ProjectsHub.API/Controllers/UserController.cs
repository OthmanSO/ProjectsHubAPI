using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProjectsHub.API.Services;
using ProjectsHub.Core;
using ProjectsHub.Exceptions;
using ProjectsHub.Model;

namespace ProjectsHub.API.Controllers
{
    [ApiController]
    [Route("/api/V1.0/user")]
    public class UserController : ControllerBase
    {
        private readonly UserService _UserService;
        private readonly IUserToken _userToken;

        public UserController(UserService userService, IUserToken usrToken)
        {
            _UserService = userService ?? throw new ArgumentNullException(nameof(UserService));
            _userToken = usrToken ?? throw new ArgumentNullException(nameof(usrToken));
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] UserAccountCreate user)
        {
            if (user == null
                || string.IsNullOrEmpty(user.FirstName)
                || string.IsNullOrEmpty(user.LastName)
                || string.IsNullOrEmpty(user.Email)
                || string.IsNullOrEmpty(user.Password))
                return BadRequest("Required feild missing");
            try
            {
                var CreatedUser = await _UserService.CreateUser(user);
                var userName = $"{CreatedUser.FirstName} {CreatedUser.LastName}";
                var tokenString = _userToken.CreateUserToken(CreatedUser._Id, userName, CreatedUser.Email);
                return Created(CreatedUser._Id, tokenString);
            }
            catch (UserAlreadyExistException)
            {
                return Conflict("User Already Exists");
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database is not connected, we are working on this!");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserAuth user)
        {
            if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
                return BadRequest("Missing Email or Password");
            try
            {
                var loggedInUser = await _UserService.GetLoggedInUser(user.Email, user.Password);
                var userName = $"{loggedInUser.FirstName} {loggedInUser.LastName}";
                var tokenString = _userToken.CreateUserToken(loggedInUser._Id, userName, loggedInUser.Email);
                return Ok(tokenString);
            }
            catch (UserPasswordNotMatchedException)
            {
                return BadRequest("Password Mismatch");
            }
            catch (ArgumentNullException)
            {
                return NotFound("User Not Found");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database is not connected, we are working on this!");
            }
        }

        [Authorize]
        [HttpPut("profilePicture")]
        public async Task<IActionResult> ChangeProfilePic([FromBody] UserprofilePictureDto ProfilePic)
        {
            if (ProfilePic.EncodedProfilePicture.IsNullOrEmpty())
            {
                return BadRequest();
            }

            var id = _userToken.GetUserIdFromToken();

            try
            {
                await _UserService.ChangeProfilePic(id, ProfilePic.EncodedProfilePicture);
                return Ok();
            }
            catch (ArgumentNullException)
            {
                return NotFound("User not found");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database is not connected, we are working on this!");
            }
        }

        [Authorize]
        [HttpPut("bio")]
        public async Task<IActionResult> ChangeBio([FromBody] BioDto UserBio)
        {
            if (UserBio.bio.IsNullOrEmpty())
            {
                return BadRequest();
            }

            var id = _userToken.GetUserIdFromToken();

            try
            {
                await _UserService.ChangeUserBio(id, UserBio.bio);
                return Ok();
            }
            catch (ArgumentNullException)
            {
                return NotFound("User not found");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database is not connected, we are working on this!");
            }
        }

        [Authorize]
        [HttpPut("Password")]
        public async Task<IActionResult> ChangePassword([FromBody] PasswordUpdateDto userPasswords)
        {
            if (userPasswords.OldPassword.IsNullOrEmpty() || userPasswords.NewPassword.IsNullOrEmpty())
            {
                return BadRequest();
            }

            var id = _userToken.GetUserIdFromToken();

            try
            {
                await _UserService.ChangeUserPassword(id, userPasswords);
                return Ok();
            }
            catch (UserPasswordNotMatchedException)
            {
                return Unauthorized("Old Password Mismatch");
            }
            catch (ArgumentNullException)
            {
                return NotFound("User Not Found");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database is not connected, we are working on this!");
            }
        }

        [Authorize]
        [HttpPut("username")]
        public async Task<IActionResult> ChangeUsername([FromBody] UserNameDto UserName)
        {
            if (UserName.FirstName.IsNullOrEmpty() || UserName.LastName.IsNullOrEmpty())
            {
                return BadRequest();
            }
            var id = _userToken.GetUserIdFromToken();
            try
            {
                await _UserService.ChangeUserName(id, UserName);
                return Ok();
            }
            catch (ArgumentNullException)
            {
                return NotFound("User not found");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database is not connected, we are working on this!");
            }
        }

        [Authorize]
        [HttpPut("Contacts")]
        public async Task<IActionResult> AddContacts([FromBody] ContactDto Contact)
        {
            if (Contact.ContactId.IsNullOrEmpty())
            {
                return BadRequest();
            }
            var id = _userToken.GetUserIdFromToken();
            try
            {
                await _UserService.AddContact(id, Contact.ContactId);
                return Ok();
            }
            catch (Exception)
            {
                return NotFound("User not found");
            }
        }

        [Authorize]
        [HttpDelete("Contacts")]
        public async Task<IActionResult> DeleteContacts([FromBody] ContactDto Contact)
        {
            if (Contact.ContactId.IsNullOrEmpty())
            {
                return BadRequest();
            }
            var id = _userToken.GetUserIdFromToken();
            try
            {
                await _UserService.DeleteContact(id, Contact.ContactId);
                return Ok();
            }
            catch (ArgumentNullException)
            {
                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database is not connected, we are working on this!");
            }
        }

        [Authorize]
        [HttpGet("Followers")]
        [HttpGet("Followers/{userId}")]
        public async Task<IActionResult> GetUserFollowers(string userId)
        {
            var id = userId ?? _userToken.GetUserIdFromToken();

            try
            {
                var listOfUsersFollowingUserAccount = await _UserService.GetListOfFollwers(id);
                return Ok(listOfUsersFollowingUserAccount);
            }
            catch (ArgumentNullException)
            {
                return NotFound("user Not Found");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database is not connected, we are working on this!");
            }
        }

        [Authorize]
        [HttpGet("Following")]
        [HttpGet("Following/{userId}")]
        public async Task<IActionResult> GetUserFollowing(string? userId)
        {
            var id = userId ?? _userToken.GetUserIdFromToken();

            try
            {
                var listOfUsersThatUserAccountFollow = await _UserService.GetListOfFollwing(id);
                return Ok(listOfUsersThatUserAccountFollow);
            }
            catch (ArgumentNullException)
            {
                return NotFound("user Not Found");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database is not connected, we are working on this!");
            }
        }



        [Authorize]
        [HttpGet("Contacts")]
        [HttpGet("Contacts/{id}")]
        public async Task<IActionResult> UserContacts(string userId)
        {
            var id = userId ?? _userToken.GetUserIdFromToken();
            try
            {
                var Contacts = await _UserService.GetUserContacts(userId);
                return Ok(Contacts);
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

        [HttpGet()]
        [HttpGet("{id}")]
        public async Task<IActionResult> userProfile(string? userId)
        {
            var id = _userToken.GetUserIdFromToken();
            try
            {
                var userProfile = await _UserService.GetUserProfileById(id);
                return Ok(userProfile);
            }
            catch (ArgumentNullException)
            {
                return NotFound("user Not Found");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database is not connected, we are working on this!");
            }
        }

        [HttpGet("shortProfile/{id}")]
        public async Task<IActionResult> UserShortProfile(string id)
        {
            try
            {
                var userShortProfile = await _UserService.GetUserShortPeofile(id);
                return Ok(userShortProfile);
            }
            catch (ArgumentNullException)
            {
                return NotFound("user Not Found");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database is not connected, we are working on this!");
            }
        }

        [Authorize]
        [HttpPut("Follow/{followUserId}")]
        public async Task<IActionResult> FollowUser(string followUserId)
        {
            var id = _userToken.GetUserIdFromToken();
            try
            {
                await _UserService.FollowUser(id, followUserId);
                return Ok();
            }
            catch (ArgumentNullException)
            {
                return NotFound("User not found");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database is not connected, we are working on this!");
            }
        }

        [Authorize]
        [HttpPut("Unfollow/{unfollowUserId}")]
        public async Task<IActionResult> UnfollowUser(string unfollowUserId)
        {
            var id = _userToken.GetUserIdFromToken();
            try
            {
                await _UserService.UnfollowUser(id, unfollowUserId);
                return Ok();
            }
            catch (ArgumentNullException)
            {
                return NotFound("User not found");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database is not connected, we are working on this!");
            }
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
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
            catch(BadEmailException) 
            {
                return BadRequest("Invalid Email");
            }
            catch (UserAlreadyExistException)
            {
                return Conflict("User Already Exists");
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.StackTrace);
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
            catch (NullReferenceException)
            {
                return NotFound("User Not Found");
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.StackTrace);
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
            catch (NullReferenceException)
            {
                return NotFound("User not found");
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.StackTrace);
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
            catch (NullReferenceException)
            {
                return NotFound("User not found");
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.StackTrace);
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
            catch (NullReferenceException)
            {
                return NotFound("User Not Found");
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.StackTrace);
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
            catch (NullReferenceException)
            {
                return NotFound("User not found");
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.StackTrace);
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
            catch (NullReferenceException)
            {
                return NotFound("user Not Found");
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.StackTrace);
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
            catch (NullReferenceException)
            {
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.StackTrace);
            }
        }

        [Authorize]
        [HttpGet("Followers/{userId}")]
        [HttpGet("Followers")]
        public async Task<IActionResult> GetUserFollowers(string? userId)
        {
            var id = userId ?? _userToken.GetUserIdFromToken();

            try
            {
                var listOfUsersFollowingUserAccount = await _UserService.GetListOfFollwers(id);
                return Ok(listOfUsersFollowingUserAccount);
            }
            catch (NullReferenceException)
            {
                return NotFound("user Not Found");
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.StackTrace);
            }
        }

        [Authorize]
        [HttpGet("Following/{userId}")]
        [HttpGet("Following")]
        public async Task<IActionResult> GetUserFollowing(string? userId)
        {
            var id = userId ?? _userToken.GetUserIdFromToken();

            try
            {
                var listOfUsersThatUserAccountFollow = await _UserService.GetListOfFollwing(id);
                return Ok(listOfUsersThatUserAccountFollow);
            }
            catch (NullReferenceException)
            {
                return NotFound("user Not Found");
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.StackTrace);
            }
        }



        [Authorize]
        [HttpGet("Contacts")]
        public async Task<IActionResult> UserContacts()
        {
            var id = _userToken.GetUserIdFromToken();
            try
            {
                var Contacts = await _UserService.GetUserContacts(id);
                return Ok(Contacts);
            }
            catch (NullReferenceException)
            {
                return NotFound("user Not Found");
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.StackTrace);
            }
        }

        [HttpGet("{userId}")]
        [HttpGet()]
        public async Task<IActionResult> userProfile(string? userId)
        {   
            try
            {
                Console.WriteLine(userId);
                var id = userId ?? _userToken.GetUserIdFromToken();
                var userProfile = await _UserService.GetUserProfileById(id);
                return Ok(userProfile);
            }
            catch (NullReferenceException)
            {
                return NotFound("user Not Found");
            }
            catch (InvalidOperationException)
            {
                return Unauthorized("user Not Logged in");
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.StackTrace);
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
            catch (NullReferenceException)
            {
                return NotFound("user Not Found");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
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
            catch (NullReferenceException)
            {
                return NotFound("User not found");
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.StackTrace);
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
            catch (NullReferenceException)
            {
                return NotFound("User not found");
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.StackTrace);
            }
        }

        [Authorize]
        [HttpGet("/api/V1.0/users/{pageNo}")]
        public async Task<ActionResult<List<UserNetworkProfile>>> GetUserNetwork(int pageNo)
        {
            if (pageNo < 1)
                pageNo = 1;

            var id = _userToken.GetUserIdFromToken();

            if (id == null)
                return Unauthorized();

            try
            {
                var listOfUserNetworkProfiles = await _UserService.GetUserNetwork(id, pageNo);
                return Ok(listOfUserNetworkProfiles);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Authorize]
        [HttpGet("/api/V1.0/users/Search/{query}/{pageNo}")]
        public async Task<ActionResult<List<UserNetworkProfile>>> SearchUserNetwork(string query, int pageNo)
        {
            if (pageNo < 1)
                pageNo = 1;

            if (query.IsNullOrEmpty())
                return Ok(new List<UserNetworkProfile>());

            var id = _userToken.GetUserIdFromToken();

            if (id == null)
                return Unauthorized();

            try
            {
                var listOfUserNetworkProfiles = await _UserService.SearchUserNetwork(query, id, pageNo);
                return Ok(listOfUserNetworkProfiles);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

    }
}

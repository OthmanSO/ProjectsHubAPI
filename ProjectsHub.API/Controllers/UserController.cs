using Microsoft.AspNetCore.Mvc;
using ProjectsHub.API.Services;
using ProjectsHub.Model;
using ProjectsHub.API.Exceptions;
using Microsoft.AspNetCore.Authorization;
using ProjectsHub.API.Model;
using Microsoft.IdentityModel.Tokens;
using ProjectsHub.Core;

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
                var userId = _UserService.CreateUser(user);
                var userName = $"{user.FirstName} {user.LastName}";
                var tokenString = _userToken.CreateUserToken(userId , userName, user.Email );
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
                var loggedInUser = _UserService.GetLoggedInUser(user.Email, user.Password);
                var userName = $"{loggedInUser.FirstName} {loggedInUser.LastName}";
                var tokenString = _userToken.CreateUserToken(loggedInUser._Id, userName, loggedInUser.Email);
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
                _UserService.ChangeProfilePic(Guid.Parse(id), ProfilePic.EncodedProfilePicture);
                return Ok();
            }
            catch (Exception e)
            {
                return NotFound("User not found");
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
                _UserService.ChangeUserBio(id, UserBio.bio); 
                return Ok();
            }
            catch (Exception e)
            {
                return NotFound("User not found");
            }
            return Ok();
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
                _UserService.ChangeUserPassword(id, userPasswords);
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

        [Authorize]
        [HttpPut("username")]
        public async Task<IActionResult> ChangeUsername([FromBody] UserNameDto UserName)
        {
            if (UserName.FirstName.IsNullOrEmpty() || UserName.LastName.IsNullOrEmpty() )
            {
                return BadRequest();
            }
            var id = _userToken.GetUserIdFromToken();
            try
            {
                _UserService.ChangeUserName(id, UserName);
                return Ok();
            }
            catch (Exception e)
            {
                return NotFound("User not found");
            }
        }

<<<<<<< HEAD
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
                _UserService.AddContact(id, Guid.Parse(Contact.ContactId));
                return Ok();
            }
            catch (FormatException e)
=======
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
            }
<<<<<<< HEAD
            catch(FormatException e)
>>>>>>> f27bcb2 (Put Contact)
=======
            catch (FormatException e)
>>>>>>> 25cae32 (formatting issues cleaning)
            {
                return BadRequest();
            }
            catch (ArgumentNullException e)
            {
                return NotFound("User not found");
            }
<<<<<<< HEAD
<<<<<<< HEAD
=======
>>>>>>> 3f00cc4 (fixed the controller in put contacts)
            catch (InvalidOperationException e)
            {
                return NotFound("user Not Found");
            }
<<<<<<< HEAD
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
                _UserService.DeleteContact(id, Guid.Parse(Contact.ContactId));
                return Ok();
            }
            catch (FormatException e)
            {
                return BadRequest();
            }
            catch (Exception e)
            {
                return Ok();
            }
        }


        [Authorize]
        [HttpGet("Contacts")]
        public async Task<IActionResult> UserContacts()
        {
            var id = _userToken.GetUserIdFromToken();

            try
            {
                var Contacts = _UserService.GetUserContacts(id);
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

        [Authorize]
=======
=======
>>>>>>> 3f00cc4 (fixed the controller in put contacts)
            return Ok();
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

        //[HttpGet()]
>>>>>>> f27bcb2 (Put Contact)
        [HttpGet("{id}")]
        public async Task<IActionResult> userProfile(string? id)
        {
            var userId = new Guid();

            if (id == null)
            {
                userId = _userToken.GetUserIdFromToken();
            }
            else
            {
                userId = Guid.Parse(id);
            }

            if (userId == Guid.Empty)
            {
                return BadRequest("Log in or include user identifier first");
            }

            var userProfile = _UserService.GetUserProfileById(userId);

            if (userProfile == null)
                return NotFound("user Not Found");
            return Ok(userProfile);
        }
<<<<<<< HEAD
        
        [HttpGet("shortProfile/{id}")]
        public async Task<IActionResult> UserShortProfile(string id)
=======

        [HttpGet("shortProfile/{id}")]
        public async Task<IActionResult> UserShortProfile(string id)
        {
            var userId = new Guid();

            try
            {
                var userShortProfile = await _UserService.GetUserShortPeofile(Guid.Parse(id), _UserRepository);
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
>>>>>>> 369edee (Get UserShortProfile)
        {
            var userId = new Guid();

            try
            {
                var userShortProfile = _UserService.GetUserShortPeofile(Guid.Parse(id));
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
    }
}

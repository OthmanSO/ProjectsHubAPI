using Microsoft.AspNetCore.Mvc;
using ProjectsHub.API.Services;
using ProjectsHub.Model;
using ProjectsHub.API.Exceptions;
using Microsoft.AspNetCore.Authorization;
using ProjectsHub.API.Model;
using ProjectsHub.Core;

namespace ProjectsHub.API.Controllers
{
    [Authorize]
    [Route("/api/V1.0/Post")]
    public class PostsController : ControllerBase
    {
        private readonly IUserToken _userToken;
        private readonly IPostService _postService;

        public PostsController (IUserToken userToken, IPostService postService)
        {
            this._userToken = userToken ?? throw new ArgumentNullException(nameof(userToken));
            this._postService = postService ?? throw new ArgumentNullException(nameof(postService));
        }

        [Authorize]
        [HttpPost()]
        public async Task<ActionResult> PostingAPost([FromBody] CreatePostDto post)
        {
            //assert post 
            if (post.Title == null || post.CoverPicture == null)
            {
                return BadRequest();
            }
            var id = _userToken.GetUserIdFromToken();
            try
            {
                var CreatedPost = await _postService.CreatePost(post, id.ToString());
                return Created(CreatedPost._id, CreatedPost);
            }
            catch (ArgumentNullException ex)
            {
                return Unauthorized();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database is not connected, we are working on this!");
            }
        }
    }
}

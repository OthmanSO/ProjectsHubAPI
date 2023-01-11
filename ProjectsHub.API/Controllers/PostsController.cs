using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectsHub.Core;
using ProjectsHub.Exceptions;
using ProjectsHub.Model;

namespace ProjectsHub.API.Controllers
{
    [Authorize]
    [Route("/api/V1.0/Post")]
    public class PostsController : ControllerBase
    {
        private readonly IUserToken _userToken;
        private readonly IPostService _postService;

        public PostsController(IUserToken userToken, IPostService postService)
        {
            this._userToken = userToken ?? throw new ArgumentNullException(nameof(userToken));
            this._postService = postService ?? throw new ArgumentNullException(nameof(postService));
        }


        [HttpPost()]
        public async Task<ActionResult> PostingAPost([FromBody] CreatePostDto post)
        {
            try
            {
                post.RemoveEmpyChunks();
                post.CleanPost();
            }
            catch (PostArssertionFailedException)
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

        [HttpGet("{id}")]
        public async Task<ActionResult<PostReturnDto>> GetPost(string id)
        {
            try
            {
                var post = await _postService.GetPost(id);
                return (post);
            }
            catch(Exception)
            {
                return NotFound();
            }
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePost(string id)
        {
            try
            {
                await _postService.DeletePost(id, _userToken.GetUserIdFromToken().ToString());
            }
            catch(UserDoesNotHavePermissionException)
            {
                return Unauthorized();
            }
            catch (Exception)
            {
                StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Ok();
        }
    }
}

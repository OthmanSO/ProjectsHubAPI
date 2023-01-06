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
        private readonly IPostService _postservice;
        public PostsController (IUserToken userToken, IPostService postService)
        {
            this._userToken = userToken ?? throw new ArgumentNullException(nameof(userToken));
            this._postservice = postService ?? throw new ArgumentNullException(nameof(postService));
        }

        [Authorize]
        [HttpPost()]
        public async Task<ActionResult> PostingAPost([FromBody] CreatePostDto post)
        {
            //assert post 
            if (post.Title != null || post.CoverPicture == null || post.PostChunks == null)
            {
                return BadRequest();
            }
            var id = _userToken.GetUserIdFromToken();
        }
    }
}

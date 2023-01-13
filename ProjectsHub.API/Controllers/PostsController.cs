﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectsHub.Core;
using ProjectsHub.Exceptions;
using ProjectsHub.Model;
using ZstdSharp.Unsafe;

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
        public async Task<IActionResult> PostingAPost([FromBody] CreatePostDto post)
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
            catch (ArgumentNullException)
            {
                return Unauthorized();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database is not connected, we are working on this!");
            }
        }

        [HttpGet("{postId}")]
        public async Task<ActionResult<PostReturnDto>> GetPost(string postid)
        {
            try
            {
                var userId = _userToken.GetUserIdFromToken();
                var post = await _postService.GetPost(userId, postid);
                return post;
            }
            catch (Exception)
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
            catch (UserDoesNotHavePermissionException)
            {
                return Unauthorized();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database is not connected, we are working on this!");
            }
            return Ok();
        }


        [HttpPut("Like/{postId}")]
        public async Task<IActionResult> LikePost(string postId)
        {
            var userId = _userToken.GetUserIdFromToken();
            try
            {
                await _postService.LikePost(userId, postId);
                return Ok();
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpPut("unLike/{postId}")]
        public async Task<IActionResult> UnLikePost(string postId)
        {
            var userId = _userToken.GetUserIdFromToken();
            try
            {
                await _postService.UnLikePost(userId, postId);
                return Ok();
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
    }
}

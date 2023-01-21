using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProjectsHub.API.Services;
using ProjectsHub.Core;
using ProjectsHub.Exceptions;
using ProjectsHub.Model;
using ZstdSharp.Unsafe;

namespace ProjectsHub.API.Controllers
{
    [Authorize]
    [Route("/api/V1.0/Project")]
    public class ProjectsController : ControllerBase
    {
        private readonly IUserToken _userToken;
        private readonly IProjectService _projectService;

        public ProjectsController(IUserToken userToken, IProjectService projectService)
        {
            this._userToken = userToken ?? throw new ArgumentNullException(nameof(userToken));
            this._projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));
        }


        [HttpPost()]
        public async Task<IActionResult> PostingAProject([FromBody] CreateProjectDto project)
        {
            if (project == null || project.Title.IsNullOrEmpty() || project.CoverPicture.IsNullOrEmpty() || project.Abstract.IsNullOrEmpty())
            {
                return BadRequest();
            }
            var id = _userToken.GetUserIdFromToken();
            try
            {
                var CreatedProject = await _projectService.CreateProject(project, id.ToString());
                return Created(CreatedProject._id, CreatedProject);
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

        [HttpGet("{projectId}")]
        public async Task<ActionResult<ProjectReturnDto>> GetProject(string projectId)
        {
            try
            {
                var userId = _userToken.GetUserIdFromToken();
                var project = await _projectService.GetProject(userId, projectId);
                return project;
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

       


        [HttpDelete("{projectId}")]
        public async Task<IActionResult> DeleteProject(string projectId)
        {
            try
            {
                await _projectService.DeleteProject(projectId, _userToken.GetUserIdFromToken().ToString());
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


        [HttpPut("Like/{projectId}")]
        public async Task<IActionResult> LikeProject(string projectId)
        {
            var userId = _userToken.GetUserIdFromToken();
            try
            {
                await _projectService.LikeProject(userId, projectId);
                return Ok();
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpPut("unLike/{projectId}")]
        public async Task<IActionResult> UnLikePost(string projectId)
        {
            var userId = _userToken.GetUserIdFromToken();
            try
            {
                await _projectService.UnLikeProject(userId, projectId);
                return Ok();
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpGet("{projectId}/ShortProject")]
        public async Task<ActionResult<ShortProject>> GetShortProject(string projectId)
        {
            var userId = _userToken.GetUserIdFromToken();
            try
            {
                var shortProject = await _projectService.GetShortProject(userId, projectId);
                return Ok(shortProject);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return NotFound();
            }
        }


        [HttpGet("Projects")]
        [HttpGet("Projects/{userId}")]
        public async Task<ActionResult<List<ShortProject>>> GetUserPosts(string userId)
        {
            var userLoggedIn = _userToken.GetUserIdFromToken();
            var userWantedProjectsList = userId ?? userLoggedIn;
            try
            {
                var listOfUsetProjects = await _projectService.GetUserProjectList(userLoggedIn, userWantedProjectsList);
                return Ok(listOfUsetProjects);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
    }
}

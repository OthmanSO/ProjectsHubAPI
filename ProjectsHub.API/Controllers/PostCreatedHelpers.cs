using Microsoft.IdentityModel.Tokens;
using ProjectsHub.Exceptions;
using ProjectsHub.Model;
using System.Text.RegularExpressions;

namespace ProjectsHub.API.Controllers
{
    internal static class PostCreatedHelpers
    {

        internal static void CleanPost(this CreatePostDto post)
        {
            if (post.Title.IsNullOrEmpty() || post.CoverPicture.IsNullOrEmpty() || post.PostChunks.IsNullOrEmpty())
            {
                throw new PostArssertionFailedException();
            }
        }

        internal static void RemoveEmpyChunks(this CreatePostDto post)
        {
            if (post.PostChunks.IsNullOrEmpty())
            {
                throw new PostArssertionFailedException();
            }

            foreach (var chunk in post.PostChunks)
            {
                if (chunk.ChunkType == null || chunk.Body.IsNullOrEmpty())
                {
                    post.PostChunks.Remove(chunk);
                }
            }
        }
    }
}
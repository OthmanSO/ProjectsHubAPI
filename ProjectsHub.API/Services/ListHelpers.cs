using ProjectsHub.Model;

namespace ProjectsHub.API.Services
{
    public static class ListHelpers
    {
        public static int GetLastComment(this List<Comment> comments) =>
            comments.OrderBy(c => c.Id)
                    .ToList()
                    .TakeLast(1)
                    .First().Id;
        
    }
}

using ProjectsHub.Model;

namespace ProjectsHub.API.Services
{
    public static class ListHelpers
    {
        public static int GetLastComment(this List<Comment> comments)
        {
            try
            {
                return comments.OrderBy(c => c.Id)
                    .ToList()
                    .TakeLast(1)
                    .First().Id;
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}

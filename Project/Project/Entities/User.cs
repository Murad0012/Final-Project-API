using Microsoft.AspNetCore.Identity;

namespace Project.Entities
{
    public class User : IdentityUser
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ProfileImg { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public List<Post> Posts { get; set; }
        public List<Comment> Comments { get; set; }
    }
}

using Microsoft.AspNetCore.Identity;

namespace Project.Entities
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public string ProfileImg { get; set; }
        public string Description { get; set; }

        public List<Post> Posts { get; set; }
        public List<Comment> Comments { get; set; }
    }
}

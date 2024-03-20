using Project.DTO_s.Post;

namespace Project.DTO_s.Account
{
    public class UserDetailedGetDto
    {
        public string UserName { get; set; }
        public string ProfileImg { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public List<PostDto> Posts { get; set; }
    }
}

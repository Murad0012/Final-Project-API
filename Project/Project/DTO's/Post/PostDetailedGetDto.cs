using Project.DTO_s.Comment;

namespace Project.DTO_s.Post
{
    public class PostDetailedGetDto
    {
        public string Caption { get; set; }
        public string Img { get; set; }
        public string Tags { get; set; }
        public DateTime CreatedAt { get; set; }

        public string UserName { get; set; }

        public List<CommentGetDto> Comments { get; set; }
    }
}

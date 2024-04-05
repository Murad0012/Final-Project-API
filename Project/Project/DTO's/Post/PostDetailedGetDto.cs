using Project.DTO_s.Comment;
using Project.DTO_s.Like;

namespace Project.DTO_s.Post
{
    public class PostDetailedGetDto
    {
        public int Id { get; set; }
        public string Caption { get; set; }
        public string Img { get; set; }
        public string Tags { get; set; }
        public DateTime CreatedAt { get; set; }
        public int LikeCount { get; set; }


        public string UserName { get; set; }
        public string UserProfileImg { get; set; }
        public string UserId { get; set; }

        public List<CommentGetDto> Comments { get; set; }

    }
}

using Project.Entities;

namespace Project.DTO_s.Post
{
    public class PostGetDto
    {
        public int Id { get; set; }
        public string Caption { get; set; }
        public string Img { get; set; }
        public string Tags { get; set; }
        public DateTime CreatedAt { get; set; }

        public string UserUserName { get; set; }
        public string UserProfileImg { get; set; }

    }
}

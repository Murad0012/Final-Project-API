namespace Project.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public string Caption { get; set; }
        public string Img { get; set; }
        public string Tags { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string UserId { get; set; }
        public User User { get; set; }

        public List<Comment> Comments { get; set; }
    }
}

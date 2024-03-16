namespace Project.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public string Caption { get; set; }
        public string Img { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<Comment> Comments { get; set; }
    }
}

namespace Project.DTO_s.Post
{
    public class PostDto
    {
        public string Caption { get; set; }
        public IFormFile Img { get; set; }
        public string Tags { get; set; }
        public string UserId { get; set; }
    }
}

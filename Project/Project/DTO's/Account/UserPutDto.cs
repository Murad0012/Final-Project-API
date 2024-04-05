namespace Project.DTO_s.Account
{
    public class UserPutDto
    {
        public string Name { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Description { get; set; }
        public IFormFile ProfileImg { get; set; }
    }
}

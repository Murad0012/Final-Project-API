using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.Data;
using Project.DTO_s.Comment;
using Project.Entities;
using System.IdentityModel.Tokens.Jwt;

namespace Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _dbContext;
        public CommentController(IHttpContextAccessor httpContextAccessor, AppDbContext dbContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
        }

        [HttpPost("AddComment")]
        public IActionResult AddComment(int postId,[FromForm]CommentPostDto dto)
        {
            var comment = new Comment
            {
                UserId = GetLoggedUserId(),
                PostId = postId,
                Description = dto.Description,
            };

            _dbContext.Comments.Add(comment);
            _dbContext.SaveChanges();

            return Ok();
        }

        //JWT Token Method
        private string GetLoggedUserId()
        {
            var accessToken = _httpContextAccessor.HttpContext!.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(accessToken);

            var userIdClaim = token.Claims.FirstOrDefault(c => c.Type == "UserID");

            return userIdClaim!.Value;
        }
    }
}

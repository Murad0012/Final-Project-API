using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.DTO_s.Comment;
using Project.DTO_s.Post;
using Project.Entities;
using System.IdentityModel.Tokens.Jwt;

namespace Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikeController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public LikeController(IHttpContextAccessor httpContextAccessor, AppDbContext dbContext, IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        [HttpPost("AddLike")]
        public async Task<IActionResult> AddLike(int postId)
        {
            var like = new Like
            {
                UserId = GetLoggedUserId(),
                PostId = postId,
            };

            var foundedLike = await _dbContext.Likes
                .Where(l => l.PostId == postId && l.UserId == GetLoggedUserId())
                .FirstOrDefaultAsync();

            if (foundedLike != null) { return BadRequest(); }

            _dbContext.Likes.Add(like);
            _dbContext.SaveChanges();

            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> UnlikePost(int postId)
        {
            var like = await _dbContext.Likes
                .Where(l => l.PostId == postId && l.UserId == GetLoggedUserId())
                .FirstOrDefaultAsync();

            if (like == null) { return NotFound(); }

            _dbContext.Likes.Remove(like);
            await _dbContext.SaveChangesAsync();

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

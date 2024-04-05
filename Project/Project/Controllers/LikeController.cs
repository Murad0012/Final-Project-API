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

        [HttpGet("CheckFollow")]
        public async Task<bool> CheckFollow(int postId, string userId)
        {
            var existingLike = await _dbContext.Likes
             .FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);

            if (existingLike == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        [HttpPost("Like")]
        public async Task<IActionResult> Like(int postId, string userId)
        {
            var like = new Like
            {
                UserId = userId,
                PostId = postId,
            };

            var foundedLike = await _dbContext.Likes
                .Where(l => l.PostId == postId && l.UserId == userId)
                .FirstOrDefaultAsync();

            if (foundedLike != null) { return BadRequest(); }

            _dbContext.Likes.Add(like);
            _dbContext.SaveChanges();

            return Ok();
        }

        [HttpDelete("Unlike")]
        public async Task<IActionResult> Unlike(int postId, string userId)
        {
            var like = await _dbContext.Likes
                .Where(l => l.PostId == postId && l.UserId == userId)
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

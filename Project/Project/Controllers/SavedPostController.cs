using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.DTO_s.Post;
using Project.Entities;
using System.IdentityModel.Tokens.Jwt;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SavedPostController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public SavedPostController(IHttpContextAccessor httpContextAccessor, AppDbContext dbContext, IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        [HttpGet("GetSavedPosts")]
        public async Task<IActionResult> GetSavedPosts()
        {
            var userIds = await _dbContext.SavedPosts
            .Where(r => r.UserId == GetLoggedUserId())
            .Select(r => r.PostId)
            .ToListAsync();

            var res = await _dbContext.Posts.Include(x => x.User).Include(x => x.Comments)
                .Where(post => userIds.Contains(post.Id)).Select(x => _mapper.Map<Post, PostGetDto>(x))
                .ToListAsync();

            return Ok(res);
        }

        [HttpGet("SortedByDateTime")]
        public async Task<IActionResult> SortedByDateTime()
        {
            var userId = GetLoggedUserId();

            var res = await _dbContext.SavedPosts
                .Where(sp => sp.UserId == userId)
                .OrderByDescending(sp => sp.SavedAt)
                .Include(sp => sp.Post)
                    .ThenInclude(p => p.User)
                .Select(sp => _mapper.Map<Post, PostGetDto>(sp.Post))
                .ToListAsync();

            return Ok(res);
        }

        [HttpPost("Save")]
        public async Task<IActionResult> Save(int postId)
        {
            var SavedPost = new SavedPost
            {
                UserId = GetLoggedUserId(),
                PostId = postId,
            };

            var savedPost = await _dbContext.SavedPosts
                .Where(l => l.PostId == postId && l.UserId == GetLoggedUserId())
                .FirstOrDefaultAsync();

            if (savedPost != null) { return BadRequest(); }

            _dbContext.SavedPosts.Add(SavedPost);
            _dbContext.SaveChanges();

            return Ok();
        }

        [HttpDelete("Unsave")]
        public async Task<IActionResult> Unsave(int postId)
        {
            var savedPost = await _dbContext.SavedPosts
                .Where(l => l.PostId == postId && l.UserId == GetLoggedUserId())
                .FirstOrDefaultAsync();

            if (savedPost == null) { return NotFound(); }

            _dbContext.SavedPosts.Remove(savedPost);
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

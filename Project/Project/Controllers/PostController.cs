    using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.DTO_s.Post;
using Project.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public PostController(IHttpContextAccessor httpContextAccessor, AppDbContext dbContext,IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        [HttpGet("GetPosts")]
        public IActionResult GetPosts()
        {
            var posts = _dbContext.Posts.Include(x => x.User)
                                          .AsNoTracking()
                                          .Select(x => _mapper.Map<Post, PostGetDto>(x))
                                          .ToList();

            var random = new Random();
            posts = posts.OrderBy(x => random.Next()).ToList();

            return Ok(posts);
        }

        [HttpGet("GetPost/{id}")]
        public IActionResult GetPost(int id)
        {
            var post = _dbContext.Posts
                                  .Include(x => x.User)
                                  .Include(x => x.Comments)
                                  .Include(x => x.Likes)
                                      .ThenInclude(x => x.User)
                                  .FirstOrDefault(x => x.Id == id);

            if (post is null)
                return NotFound();

            Console.WriteLine(post.Likes.Count());

            var dto = _mapper.Map<Post, PostDetailedGetDto>(post);

            dto.LikeCount = post.Likes.Count();

            return Ok(dto);
        }

        [HttpGet("FollowingUserPosts")]
        public async Task<IActionResult> FollowingUsersPost()
        {
            var followedUserIds = await _dbContext.Relationships
            .Where(r => r.FollowerId == GetLoggedUserId())
            .Select(r => r.FollowingId)
            .ToListAsync();

            var followedUsersPosts = await _dbContext.Posts.Include(x => x.User).Include(x => x.Comments)
                .Where(post => followedUserIds.Contains(post.UserId)).Select(x => _mapper.Map<Post, PostGetDto>(x))
                .ToListAsync();

            return Ok(followedUsersPosts);
        }

        [HttpGet("Explore/{id}")]
        public async Task<IActionResult> Explore(string id)
        {
            var followedUserIds = await _dbContext.Relationships
            .Where(r => r.FollowerId == id)
            .Select(r => r.FollowingId)
            .ToListAsync();

            var followedUsersPosts = await _dbContext.Posts.Include(x => x.User).Include(x => x.Comments)
                .Where(post => !followedUserIds.Contains(post.UserId)).Select(x => _mapper.Map<Post, PostGetDto>(x))
                .ToListAsync();

            return Ok(followedUsersPosts);
        }

        [HttpPost("CreatePost")]
        public IActionResult CreatePost([FromForm] PostDto dto)
        {
            var post = new Post
            {
                UserId = dto.UserId,
                Caption = dto.Caption,
                Tags = dto.Tags,
            };

            string path = Path.Combine(Directory.GetCurrentDirectory(), "Imgs");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.Img.FileName);
            string fileNameWithPath = Path.Combine(path, fileName);

            using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
            {
                dto.Img.CopyTo(stream);
            }

            post.Img = fileName;

            _dbContext.Add(post);
            _dbContext.SaveChanges();

            return Ok();
        }


        [HttpDelete("DeletePost/{id}")]
        public IActionResult DeletePost(int id)
        {
            var post = _dbContext.Posts.Include(x=>x.Comments).Include(x=>x.Likes).Include(x=>x.SavedPosts).FirstOrDefault(x => x.Id == id);

            if (post is null) return NotFound();

            _dbContext.Posts.Remove(post);
            _dbContext.SaveChanges();

            return Ok();
        }

        [HttpPut("UpdatePost")]
        public IActionResult UpdatePost([FromBody] PostPutDto dto)
        {
            var post = _dbContext.Posts.FirstOrDefault(x=>x.Id == dto.Id);
            if (post is null) return NotFound();
            
            _mapper.Map(dto, post);

            _dbContext.Update(post);
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

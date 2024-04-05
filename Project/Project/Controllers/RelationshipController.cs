using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Project.Data;
using Project.DTO_s.Account;
using Project.DTO_s.Post;
using Project.DTO_s.Relationship;
using Project.Entities;
using System.IdentityModel.Tokens.Jwt;

namespace Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RelationshipController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public RelationshipController(AppDbContext dbContext, UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        [HttpGet("ShowUserFollowers")]
        public async Task<IActionResult> ShowUserFollowers(string userId)
        {
            var users = await _dbContext.Relationships
                        .Where(r => r.FollowingId == userId)
                        .Select(r => r.Follower)
                        .ToListAsync();

            if (users == null) return NotFound();

            var followersDto = users.Select(follower => new UserGetDto
            {
                Id = follower.Id,
                UserName = follower.UserName,
            });

            return Ok(followersDto);
        }

        [HttpGet("ShowUserFollowings")]
        public async Task<IActionResult> ShowUserFollowings(string userId)
        {
            var users = await _dbContext.Relationships
                .Where(r => r.FollowerId == userId)
                .Select(r => r.Following)
                .ToListAsync();

            if (users == null || !users.Any())
                return NotFound();

            var followersDto = users.Select(follower => new UserGetDto
            {
                Id = follower.Id,
                UserName = follower.UserName,
            });

            return Ok(followersDto);
        }

        [HttpGet("GetNonFollowedUsers/{id}")]
        public async Task<IActionResult> GetNonFollowedUsers(string id)
        {
            var allUsers = await _dbContext.Users.ToListAsync();

            var followedUserIds = await _dbContext.Relationships
                .Where(r => r.FollowerId == id)
                .Select(r => r.FollowingId)
                .ToListAsync();

            var nonFollowedUsers = allUsers
                .Where(u => !followedUserIds.Contains(u.Id))
                .Select(u => new UserGetDto
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    ProfileImg = u.ProfileImg
                })
                .ToList();

            var randomizedNonFollowedUsers = nonFollowedUsers
                .Where(x => x.Id != id)
                .OrderBy(x => Guid.NewGuid())
                .Take(3) 
                .ToList();

            return Ok(randomizedNonFollowedUsers);
        }

        [HttpGet("CheckFollow")]
        public async Task<bool> CheckFollow(string followedId, string userId)
        {
            var existingRelationship = await _dbContext.Relationships
            .FirstOrDefaultAsync(r => r.FollowerId == userId && r.FollowingId == followedId);

            if (existingRelationship == null)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        [HttpPost("Follow")]
        public async Task<IActionResult> Follow(string followedId,string userId)
        {

            if(followedId == userId) return Conflict("You can't follow yourself.");

            var existingRelationship = await _dbContext.Relationships
            .FirstOrDefaultAsync(r => r.FollowerId == userId && r.FollowingId == followedId);

            if (existingRelationship != null) return Conflict("Relationship already exists.");

            var newRelationship = new Relationship
            {
                FollowerId = userId,
                FollowingId = followedId
            };

            _dbContext.Relationships.Add(newRelationship);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("Unfollow")]
        public async Task<IActionResult> Unfollow(string followedId, string userId)
        {
            var relationship = await _dbContext.Relationships
                .FirstOrDefaultAsync(r => r.FollowerId == userId && r.FollowingId == followedId);

            if (relationship == null)
            {
                return NotFound("Relationship not found.");
            }

            _dbContext.Relationships.Remove(relationship);
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

using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.DTO_s.Account;
using Project.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        public SearchController(IHttpContextAccessor httpContextAccessor, AppDbContext dbContext,IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        [HttpGet("SearchUser")]
        public async Task<ActionResult> Search(string username, string filterType, string userId)
        {
            if (username == "." && filterType == "all")
            {
                var users = await _dbContext.Users.Select(x => _mapper.Map<User, UserGetDto>(x)).ToListAsync();

                return Ok(users);
            }

            if (username == "." && filterType == "following")
            {
                var users = await _dbContext.Relationships
                .Where(r => r.FollowerId == userId)
                .Select(r => r.Following)
                .ToListAsync();

                var followersDto = users.Select(follower => new UserGetDto
                {
                    Id = follower.Id,
                    UserName = follower.UserName,
                    ProfileImg = follower.ProfileImg
                });

                return Ok(followersDto);
            }

            if (username == "." && filterType == "follows")
            {
                var users = await _dbContext.Relationships
                        .Where(r => r.FollowingId == userId)
                        .Select(r => r.Follower)
                        .ToListAsync();

                var followersDto = users.Select(follower => new UserGetDto
                {
                    Id = follower.Id,
                    UserName = follower.UserName,
                    ProfileImg = follower.ProfileImg
                });

                return Ok(followersDto);
            }

            if (filterType == "all")
            {
                var users = await _dbContext.Users
                    .Where(u => u.UserName.Contains(username))
                    .Select(x => _mapper.Map<User, UserGetDto>(x))
                    .ToListAsync();

                return Ok(users);
            }
            else if (filterType == "following")
            {
                var followedUserIds = await _dbContext.Relationships
                    .Where(r => r.FollowerId == userId)
                    .Select(r => r.FollowingId)
                    .ToListAsync();

                var users = await _dbContext.Users
                    .Where(u => followedUserIds.Contains(u.Id) && u.UserName.Contains(username))
                    .Select(x => _mapper.Map<User, UserGetDto>(x))
                    .ToListAsync();

                return Ok(users);
            }
            else if (filterType == "follows")
            {
                var followers = await _dbContext.Relationships
                .Where(r => r.FollowingId == userId)
                .Select(r => r.Follower).Where(f => f.UserName.Contains(username))
                .ToListAsync();

                var users =  followers
                    .Select(follower => new UserGetDto
                    {
                        Id = follower.Id,
                        UserName = follower.UserName,
                        ProfileImg = follower.ProfileImg
                    });

                return Ok(users);
            }
            else
            {
                return BadRequest("Invalid filter type.");
            }
        }


    }
}

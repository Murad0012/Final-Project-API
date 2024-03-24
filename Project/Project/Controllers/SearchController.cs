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

        [HttpGet("Search")]
        public async Task<ActionResult> Search(string username)
        {
            var users = await _dbContext.Users
            .Where(u => u.UserName.Contains(username)).Select(x => _mapper.Map<User, UserGetDto>(x))
            .ToListAsync();

            return Ok(users);
        }
    }
}

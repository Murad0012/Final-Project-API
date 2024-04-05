using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Project.Data;
using Project.DTO_s.Account;
using Project.DTO_s.Post;
using Project.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public AccountController(AppDbContext dbContext,UserManager<User> userManager,SignInManager<User> signInManager, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _signInManager.PasswordSignInAsync(dto.UserName, dto.Password,false,false);

            if (!result.Succeeded) return BadRequest(result.ToString());

            var user = _dbContext.Users.FirstOrDefault(x=>x.UserName == dto.UserName);

            var token = GetToken(user.Id);

            return Ok(new UserInfoDto
            {
                Token = token,
                UserName = dto.UserName,    
            });
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var newUser = new User
            {
                UserName = dto.UserName,
                Email = dto.Email,  
                Name = dto.Name
            };

            var result = await _userManager.CreateAsync(newUser ,dto.Password);

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(newUser.Id);
        }

        [HttpGet("GetUsers")]
        public IActionResult GetUsers()
        {
            var users = _dbContext.Users.AsNoTracking().Select(x => _mapper.Map<User, UserGetDto>(x)).ToList();

            return Ok(users);
        }

        [HttpGet("GetUserDetailes/{id}")]
        public async Task<IActionResult> GetUserDetailes(string id)
        {
            var user = _dbContext.Users.Include(x=>x.Posts).FirstOrDefault(x => x.Id == id);

            if (user is null) return NotFound();

            var dto = _mapper.Map<User, UserDetailedGetDto>(user);

            dto.Posts.Reverse();

            var following = await _dbContext.Relationships
                .Where(r => r.FollowerId == id)
                .Select(r => r.Following)
                .ToListAsync();

            dto.FollowingCount = following.Count;

            var follows = await _dbContext.Relationships
                        .Where(r => r.FollowingId == id)
                        .Select(r => r.Follower)
                        .ToListAsync();

            dto.FollowsCount = follows.Count;

            return Ok(dto);
        }

        [HttpPut("UpdateUser")]
        public IActionResult UpdateUser([FromForm] UserPutDto dto)
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.Id == dto.UserId);

            if (user == null)
            {
                return NotFound();
            }

            user.Name = dto.Name;
            user.UserName = dto.UserName;
            user.Description = dto.Description;
 
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Imgs");
             
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.ProfileImg.FileName);
            string fileNameWithPath = Path.Combine(path, fileName);

            if (Directory.GetFiles(path, fileName).Length > 0)
            {
                return BadRequest("File with the same name already exists.");
            }
            else
            {
                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    dto.ProfileImg.CopyTo(stream);
                }
                user.ProfileImg = fileName;
            }

            _dbContext.Update(user);
            _dbContext.SaveChanges();

            return Ok();
        }


        [HttpDelete("DeleteUser/{id}")]
        public IActionResult DeleteUser(string id)
        {
            var user = _dbContext.Users.Include(x => x.Comments).FirstOrDefault(x => x.Id == id);

            if (user is null) return NotFound();

            _dbContext.Users.Remove(user);
            _dbContext.SaveChanges();

            return Ok();
        }

        //JWT Token Methods
        private string GetToken(string Id)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                            issuer: _configuration["JWT:ValidIssuer"],
                            audience: _configuration["JWT:ValidAudience"],
                            expires: DateTime.Now.AddHours(1),
                            claims: new List<Claim> { new Claim("UserID",Id) },
                            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GetLoggedUserId()
        {
            var accessToken = _httpContextAccessor.HttpContext!.Request.Headers["Authorization"].ToString().Replace("Bearer", "");

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(accessToken);

            var userIdClaim = token.Claims.FirstOrDefault(c => c.Type == "UserID");

            return userIdClaim!.Value;
        }
    }
}

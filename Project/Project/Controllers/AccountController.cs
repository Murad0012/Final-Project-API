using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project.DTO_s.Account;
using Project.Entities;

namespace Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager,SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromForm] LoginDto dto)
        {
            var result = await _signInManager.PasswordSignInAsync(dto.UserName, dto.Password,false,false);

            if (!result.Succeeded) return BadRequest(result.ToString());

            return Ok();
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromForm] RegisterDto dto)
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
    }
}

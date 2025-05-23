using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WebAPI.Models;
using WebAPI.Services;

namespace WebAPI.Controllers;



[Route("api/[controller]")]
[ApiController]
public class AccountController(AccountService accountService, IConfiguration configuration) : Controller
{

    private readonly AccountService _accountService = accountService;
    private readonly IConfiguration _configuration = configuration;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {

        if (registerDto == null)
        {
            return BadRequest("Invalid registration data.");
        }
        var result = await _accountService.RegisterUserAsync(registerDto);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return Ok("User registered successfully.");

    }




    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] RegisterDto registerDto)
    {
        var validUser = await _accountService.SignInAsync(registerDto.Email, registerDto.Password);
        if (validUser == null)
        
            return Unauthorized("Invalid credentials.");
        
        var token = _accountService.GenerateJwtToken(validUser, _configuration);
        return Ok(new { token });
    }


    [HttpGet("getuser/{id}")]
    public async Task<IActionResult> GetUser(string id)
    {
        var user = await _accountService.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound("User not found.");
        }
        return Ok(user);
    }
}

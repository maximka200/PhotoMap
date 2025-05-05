using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using PhotoMapAPI.Controllers;
using PhotoMapAPI.DTOs;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _config;
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> logger;

    public AuthController(
        UserManager<User> userManager,
        IConfiguration config,
        IAuthService authService)
    {
        _userManager = userManager;
        _config = config;
        _authService = authService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = await _userManager.FindByNameAsync(dto.UserName);
        try
        {
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                logger.Log(LogLevel.Information, "Invalid login attempt for user {UserName}", dto.UserName);
                return Unauthorized("invalid username or password");
            }

            var token = await _authService.GenerateJwtTokenAsync(user);
            return Ok(new
            {
                token,
                user.Id
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during login for user {UserName}", dto.UserName);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            UserName = dto.UserName,
            Email = dto.Email
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description);
            return BadRequest(new { Errors = errors });
        }
        
        var token = await _authService.GenerateJwtTokenAsync(user);
        return Ok(new
        {
            token,
            user.Id
        });
    }
}
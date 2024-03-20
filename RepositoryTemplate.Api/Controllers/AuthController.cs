using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepositoryTemplate.Api.Services.Interfaces;
using RepositoryTemplate.Data.Dtos.Requests.Auth;
using RepositoryTemplate.Repository.Repositories.Interfaces;

namespace RepositoryTemplate.Api.Controllers;

public class AuthController : BaseController
{
    private readonly IAuthService _authService;
    
    public AuthController(IUnitOfWork unitOfWork, IMapper mapper, IAuthService authService) : base(unitOfWork, mapper)
    {
        _authService = authService;
    }
    
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        var loginOperation = await _authService.LoginUser(request);
        
        if (!loginOperation.Success)
            return BadRequest(loginOperation);

        return Ok(loginOperation);
    }
    
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegistrationRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        var registrationOperation = await _authService.RegisterUser(request);
        
        if (!registrationOperation.Success)
            return BadRequest(registrationOperation);
        
        return Ok(registrationOperation);
    }
    
    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] UserRefreshTokenRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest();
        
        var refreshOperation = await _authService.RefreshToken(request);
        
        if (!refreshOperation.Success)
            return BadRequest(refreshOperation);
        
        return Ok(refreshOperation);
    }
    
    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] UserChangePasswordRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest();
        
        // Get user from httpcontext
        var user = HttpContext.User;
        var changePasswordOperation = await _authService.ChangePassword(request, null);
        
        if (!changePasswordOperation)
            return BadRequest(false);

        return Ok(true);
    }
}
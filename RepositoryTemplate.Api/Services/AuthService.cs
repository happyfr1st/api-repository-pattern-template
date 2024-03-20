using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using RepositoryTemplate.Api.Services.Interfaces;
using RepositoryTemplate.Data.Dtos.Requests.Auth;
using RepositoryTemplate.Data.Dtos.Responses;
using RepositoryTemplate.Data.Entities;
using RepositoryTemplate.Repository.Repositories.Interfaces;

namespace RepositoryTemplate.Api.Services;

/// <inheritdoc/>
public class AuthService : IAuthService
{
    private readonly ILogger<AuthService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    
    public AuthService(
        ILogger<AuthService> logger, 
        IUnitOfWork unitOfWork,
        UserManager<User> userManager, 
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }

    /// <inheritdoc/>
    public async Task<AuthResponse> LoginUser(UserLoginRequest request)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);

        if (existingUser == null)
        {
            return new AuthResponse(false, new List<string>() { "User with this email does not exist" });
        }

        var isPasswordCorrect = await _userManager.CheckPasswordAsync(existingUser, request.Password);

        if (!isPasswordCorrect)
        {
            return new AuthResponse(false, new List<string>() { "Invalid password" });
        }

        return await GenerateJwtToken(existingUser);
    }

    /// <inheritdoc/>
    public async Task<AuthResponse> RegisterUser(UserRegistrationRequest request)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);

        if (existingUser != null)
        {
            return new AuthResponse(false, new List<string>() { "User with this email already exists" });
        }
        
        var newUser = new User() { Email = request.Email, UserName = request.Username };
        
        var isCreated = await _userManager.CreateAsync(newUser, request.Password);

        if (isCreated.Succeeded)
        {
            await _userManager.AddToRoleAsync(newUser, "User");

            return await GenerateJwtToken(newUser);
        }

        return new AuthResponse(false, new List<string>() { "Failed to create user" });
    }

    /// <inheritdoc/>
    public async Task<AuthResponse> RefreshToken(UserRefreshTokenRequest request)
    {
        return await VerifyAndGenerateToken(request);
    }

    /// <inheritdoc/>
    public async Task<bool> ChangePassword(UserChangePasswordRequest request, User existingUser)
    {
        var isChanged = await _userManager.ChangePasswordAsync(existingUser, request.OldPassword, request.NewPassword);

        if (isChanged.Succeeded)
        {
            return true;
        }

        return false;
    }

    private async Task<AuthResponse> GenerateJwtToken(User user)
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        
        var secret = _configuration.GetSection("JwtSettings:Secret").Value!;
        var key = Encoding.UTF8.GetBytes(secret);

        var claims = await GetAllValidClaims(user);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration.GetSection("JwtSettings:TokenLifetimeInMinutes").Value)),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = jwtTokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = jwtTokenHandler.WriteToken(token);

        var refreshToken = new RefreshToken()
        {
            JwtId = token.Id,
            IsUsed = false,
            IsRevoked = false,
            UserId = user.Id,
            AddedDate = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddMonths(3),
            Token = RandomString(35) + Guid.NewGuid()
        };
        
        await _unitOfWork.RefreshTokens.Add(refreshToken);
        await _unitOfWork.CompleteAsync();
        
        return new AuthResponse(jwtToken, refreshToken.Token, true);
    }

    private async Task<List<Claim>> GetAllValidClaims(User user)
    {
        var claims = new List<Claim>
        {
            new Claim("Id", user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Get the claims which are assigned to the user
        var userClaims = await _userManager.GetClaimsAsync(user);
        claims.AddRange(userClaims);

        // Get the user role and add it to the claims
        var userRoles = await _userManager.GetRolesAsync(user);

        foreach (var userRole in userRoles)
        {
            var role = await _roleManager.FindByNameAsync(userRole);

            if (role != null)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));

                var roleClaims = await _roleManager.GetClaimsAsync(role);
                foreach (var roleClaim in roleClaims)
                {
                    claims.Add(roleClaim);
                }
            }
        }

        return claims;
    }

    private async Task<AuthResponse> VerifyAndGenerateToken(UserRefreshTokenRequest tokenRequest)
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        
        var secret = _configuration.GetSection("JwtSettings:Secret").Value!;
        var key = Encoding.UTF8.GetBytes(secret);

        var tokenValidationsParameters = new TokenValidationParameters()
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateLifetime = false
        };

        try
        {
            // Validation 1 - Validate JWT token format
            var tokenInVerification = jwtTokenHandler.ValidateToken(tokenRequest.AccessToken, tokenValidationsParameters, out var validatedToken);

            // Validation 2 - Validate Encryption
            if (validatedToken is JwtSecurityToken jwtSecurityToken)
            {
                var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);

                if (result == false)
                {
                    return null;
                }
            }
            
            /*
            // Validation 3 - Validate Expiry Date
            var utcExpiryDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

            var expiryDate = UnixTimeStampToDateTime(utcExpiryDate);

            if (expiryDate > DateTime.UtcNow)
            {
                return new AuthResponse(false, new List<string>() { "AccessToken has not expired yet" });
            }
            */

            // Validation 4 - Validate Existence of the token
            var storedToken = await _unitOfWork.RefreshTokens.GetByRefreshToken(tokenRequest.RefreshToken);

            if (storedToken == null)
            {
                return new AuthResponse(false, new List<string>() { "RefreshToken does not exist" });
            }

            // Validation 5 - Validate if its used
            if (storedToken.IsUsed)
            {
                return new AuthResponse(false, new List<string>() { "RefreshToken has been used" });
            }

            // Validation 6 - Validate if revoke
            if (storedToken.IsRevoked)
            {
                return new AuthResponse(false, new List<string>() { "RefreshToken has been revoked" });
            }

            // Validation 7 - Validate the id
            var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            if (storedToken.JwtId != jti)
            {
                return new AuthResponse(false, new List<string>() { "AccessToken signature does not match" });
            }

            // update current token
            storedToken.IsUsed = true;
            
            _unitOfWork.RefreshTokens.Update(storedToken);
            await _unitOfWork.CompleteAsync();

            // generate new token
            var tokenUser = await _userManager.FindByIdAsync(storedToken.UserId);
            return await GenerateJwtToken(tokenUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while parsing Token");
            return new AuthResponse(false, new List<string>() { "Error while parsing Token" });
        }
    }

    private DateTime UnixTimeStampToDateTime(long unixTimeStamp)
    {
        var dateTimeVal = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTimeVal = dateTimeVal.AddSeconds(unixTimeStamp).ToUniversalTime();

        return dateTimeVal;
    }

    private string RandomString(int length)
    {
        var random = new Random();
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        return new string(Enumerable.Repeat(chars, length)
            .Select(x => x[random.Next(x.Length)]).ToArray());
    }
}
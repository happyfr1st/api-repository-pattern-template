using RepositoryTemplate.Data.Dtos.Requests.Auth;
using RepositoryTemplate.Data.Dtos.Responses;
using RepositoryTemplate.Data.Entities;

namespace RepositoryTemplate.Api.Services.Interfaces;

/// <summary>
/// Service for handling authentication and authorization
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Login existing user
    /// </summary>
    /// <param name="user login request"></param>
    /// <returns>auth response</returns>
    Task<AuthResponse> LoginUser(UserLoginRequest request);
    
    /// <summary>
    /// Register new user
    /// </summary>
    /// <param name="user registration request"></param>
    /// <returns>auth response</returns>
    Task<AuthResponse> RegisterUser(UserRegistrationRequest request);
    
    /// <summary>
    /// Takes an existing refresh token and returns a new access token and refresh token
    /// </summary>
    /// <param name="request"></param>
    /// <returns>auth response</returns>
    Task<AuthResponse> RefreshToken(UserRefreshTokenRequest request);
    
    /// <summary>
    /// Updates the password according to the passed newPassword parameter.
    /// </summary>
    /// <param name="request"></param>
    /// <returns>boolean if the operation was successful</returns>
    Task<bool> ChangePassword(UserChangePasswordRequest request, User existingUser);
}
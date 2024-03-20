namespace RepositoryTemplate.Data.Dtos.Responses;

public class AuthResponse
{
    // success, no need to add errors
    public AuthResponse(string token, string refreshToken, bool success)
    {
        Token = token;
        RefreshToken = refreshToken;
        Success = success;
        Errors = null;
    }
    
    // error, no need to add token and refresh token
    public AuthResponse(bool success, IEnumerable<string> errors)
    {
        Token = string.Empty;
        RefreshToken = string.Empty;
        Success = success;
        Errors = errors;
    }
    
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public bool Success { get; set; } 
    public IEnumerable<string>? Errors { get; set; }
}
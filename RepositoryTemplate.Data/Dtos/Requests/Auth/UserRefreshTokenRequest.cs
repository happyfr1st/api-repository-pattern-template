using System.ComponentModel.DataAnnotations;

namespace RepositoryTemplate.Data.Dtos.Requests.Auth;

public class UserRefreshTokenRequest
{
    [Required(ErrorMessage = "AccessToken is required")]
    public string AccessToken { get; set; }
    
    [Required(ErrorMessage = "Refresh token is required")]
    public string RefreshToken { get; set; }
}
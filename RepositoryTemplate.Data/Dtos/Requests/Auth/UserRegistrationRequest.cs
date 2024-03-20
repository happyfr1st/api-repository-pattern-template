using System.ComponentModel.DataAnnotations;

namespace RepositoryTemplate.Data.Dtos.Requests.Auth;

public class UserRegistrationRequest
{
    [Required(ErrorMessage = "Username is required")]
    public string? Username { get; set; }
    
    [Required(ErrorMessage = "Email is required")]
    public string? Email { get; set; }
    
    [Required(ErrorMessage = "Password is required")]
    public string? Password { get; set; }
}
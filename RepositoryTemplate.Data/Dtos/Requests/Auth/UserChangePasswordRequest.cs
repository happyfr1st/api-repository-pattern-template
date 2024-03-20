using System.ComponentModel.DataAnnotations;

namespace RepositoryTemplate.Data.Dtos.Requests.Auth;

public class UserChangePasswordRequest
{
    [Required(ErrorMessage = "Old password is required")]
    public string? OldPassword { get; set; }
    
    [Required(ErrorMessage = "New password is required")]
    public string? NewPassword { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace Pronia.ViewModels
{
    public class UserLoginVM
    {
        [Required]
        [MinLength(4, ErrorMessage = "Username or email must have minimum 4 characters!")]
        [MaxLength(80, ErrorMessage = "Username or email must have max 80 characters!")]
        public string UsernameOrEmail { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = "Password must have minimum 8 characters!")]
        [MaxLength(40, ErrorMessage = "Password must have max 40 characters!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public bool IsRemembered { get; set; }

        public bool? IsLocked { get; set; }
    }
}

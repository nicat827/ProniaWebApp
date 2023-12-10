
using Pronia.Models;
using Pronia.Utilities.Enums;
using System.ComponentModel.DataAnnotations;

namespace Pronia.ViewModels
{
    public class UserDetailsVM
    {
        [Required]
        [MinLength(3, ErrorMessage = "Name length cant be less than 3!")]
        [MaxLength(25, ErrorMessage = "Name length cant be longer than 25!")]
        public string Name { get; set; }

        [Required]
        [MinLength(3, ErrorMessage = "Surname length cant be less than 3!")]
        [MaxLength(50, ErrorMessage = "Surname length cant be less than 3!")]
        public string Surname { get; set; }

        [Required]
        [MinLength(4, ErrorMessage = "Username must have minimum 4 length!")]
        [MaxLength(30, ErrorMessage = "Username must have max 30 length!")]
        public string Username { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = "Email must have minimum 8 characters!")]
        [MaxLength(40, ErrorMessage = "Email must have max 40 characters!")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [MinLength(8)]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword))]

        public string? ConfirmNewPassword { get; set; }

        [Required]
        public Gender Gender { get; set; }

        public List<Order>? Orders { get; set; }








    }
}

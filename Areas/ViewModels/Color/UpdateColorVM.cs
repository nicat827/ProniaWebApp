using System.ComponentModel.DataAnnotations;

namespace Pronia.Areas.ViewModels
{
    public class UpdateColorVM
    {
        [Required(ErrorMessage = "Color cant be null!")]
        [MaxLength(ErrorMessage = "Value cant be longer than 25!")]
        public string Name { get; set; }
    }
}

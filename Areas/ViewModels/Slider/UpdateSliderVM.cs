using System.ComponentModel.DataAnnotations;

namespace Pronia.Areas.ViewModels
{
    public class UpdateSliderVM
    {

        [Required(ErrorMessage = "Title cant be empty!")]
        [MaxLength(ErrorMessage = "Value cant be longer than 25!")]
        public string Title { get; set; }

        [MaxLength(25, ErrorMessage = "Value cant be longer than 25!")]
        public string? SubTitle { get; set; }

        [MaxLength(200, ErrorMessage = "Value cant be longer than 200!")]

        public string? Description { get; set; }

        [Required(ErrorMessage = "Order cant be empty!")]
        public int Order { get; set; }
        public string ImageURL { get; set; }
        public IFormFile? Photo { get; set; }
    }
}

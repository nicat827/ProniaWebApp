using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pronia.Models
{
    public class Slider
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "Title cant be null!")]
        [MaxLength(ErrorMessage = "Value cant be longer than 25!")]
        public string Title { get; set; }

        [MaxLength(ErrorMessage = "Value cant be longer than 25!")]
        public string? SubTitle { get; set; }

        public string? Description { get; set; }

        public string ImageURL { get; set; }

        
        public int Order { get; set; }

        [NotMapped]
        public IFormFile Photo { get; set; }

    }
}

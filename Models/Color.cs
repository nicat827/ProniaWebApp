using System.ComponentModel.DataAnnotations;

namespace Pronia.Models
{
    public class Color
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Color cant be null!")]
        [MaxLength(ErrorMessage =  "Value cant be longer than 25!")]
        public string Name { get; set; }

        public List<ProductColor>? ProductColors { get; set; }
        
    }
}

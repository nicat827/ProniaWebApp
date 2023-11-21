using System.ComponentModel.DataAnnotations;

namespace Pronia.Models
{
    public class Size
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Size cant be null!")]
        [MaxLength(ErrorMessage = "Value cant be longer than 25!")]
        public string Type { get; set; }

        public List<ProductSize>? ProductSizes { get; set; }
    }
}

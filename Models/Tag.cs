using System.ComponentModel.DataAnnotations;

namespace Pronia.Models
{
    public class Tag
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tag cant be null!")]
        [MaxLength(ErrorMessage = "Value cant be longer than 25!")]
        public string Name { get; set; }

        public List<ProductTag>? ProductTags { get; set; }

    }
}

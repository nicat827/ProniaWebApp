using System.ComponentModel.DataAnnotations;

namespace Pronia.Areas.ViewModels
{
    public class UpdateTagVM
    {
        [Required(ErrorMessage = "Tag cant be empty!")]
        [MaxLength(ErrorMessage = "Value cant be longer than 25!")]
        public string Name { get; set; }
    }
}

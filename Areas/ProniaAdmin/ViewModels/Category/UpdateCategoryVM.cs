using System.ComponentModel.DataAnnotations;

namespace Pronia.Areas.ViewModels
{
    public class UpdateCategoryVM
    {
        [MaxLength(ErrorMessage = "Value cant be longer than 25!")]

        public string Name { get; set; }

    }
}

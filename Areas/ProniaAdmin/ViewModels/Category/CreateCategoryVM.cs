using Pronia.Models;
using System.ComponentModel.DataAnnotations;

namespace Pronia.Areas.ViewModels
{
    public class CreateCategoryVM
    {
        [MaxLength(ErrorMessage = "Value cant be longer than 25!")]

        public string Name { get; set; }

    }
}

using System.ComponentModel.DataAnnotations;

namespace Pronia.Areas.ViewModels
{
    public class CreateSizeVM
    {
       
        [MaxLength(ErrorMessage = "Value cant be longer than 25!")]
        public string Type { get; set; }

    }
}

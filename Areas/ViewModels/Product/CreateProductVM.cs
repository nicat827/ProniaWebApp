
using Pronia.Models;
using System.ComponentModel.DataAnnotations;

namespace Pronia.Areas.ViewModels
{
    public class CreateProductVM
    {
        public string Name { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Price must be bigger than 0")]
        [Required]
        public decimal? Price { get; set; }


        [Range(1, 300, ErrorMessage = "Chooce a category!")]
        public int CategoryId { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public string SKU { get; set; }

        public List<Category>? Categories { get; set; }

        //Images

        public IFormFile MainPhoto { get; set; }


        public IFormFile HoverPhoto { get; set; }

        public List<IFormFile>? OthersPhoto { get; set; }
    }
}

using Pronia.Models;
using System.ComponentModel.DataAnnotations;

namespace Pronia.Areas.ViewModels
{
    public class UpdateProductVM
    {
        public string Name { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Price must be bigger than 0")]
        [Required]
        public decimal? Price { get; set; }


        [Range(1, 300, ErrorMessage = "Chooce a category!")]
        public int CategoryId { get; set; }

        public bool IsAvilable { get; set; }

        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public string SKU { get; set; }
        public List<int>? TagIds { get; set; }
        public List<int> ColorIds { get; set; }

        public List<int> SizeIds { get; set; }
        public List<Category>? Categories { get; set; }
        public List<Tag>? Tags { get; set; }
        public List<Color>? Colors { get; set; }
        public List<Size>? Sizes { get; set; }

        //Images

        public List<ProductImage>? Images { get; set; }

        public IFormFile? MainPhoto { get; set; }


        public IFormFile? HoverPhoto { get; set; }

        public List<IFormFile>? OthersPhoto { get; set; }
    }
}

using Pronia.Utilities.Enums;

namespace Pronia.Models
{
    public class ProductImage
    {
        public int Id { get; set; }

        public string ImageURL { get; set; }


        public ImageType Type { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; }



    }
}

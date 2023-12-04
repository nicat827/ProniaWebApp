using Pronia.Models;
using Pronia.Utilities.Enums;

namespace Pronia.ViewModels
{
    public class ProductVM
    {
        public Product Product { get; set; }

        public List<Product>? SimilarProducts { get; set; }

    }
}

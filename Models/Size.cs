using System.ComponentModel.DataAnnotations;

namespace Pronia.Models
{
    public class Size
    {
        public int Id { get; set; }

        public string Type { get; set; }

        public List<ProductSize>? ProductSizes { get; set; }
    }
}

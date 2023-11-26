using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pronia.Models
{
    public class Slider
    {

        public int Id { get; set; }

        public string Title { get; set; }

        public string? SubTitle { get; set; }

        public string? Description { get; set; }

        public string ImageURL { get; set; }

        public int Order { get; set; }

    }
}

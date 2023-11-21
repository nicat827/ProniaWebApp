﻿using System.ComponentModel.DataAnnotations;

namespace Pronia.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required!")]
        [MaxLength(25, ErrorMessage ="Name cant be longer than 25!")]
        public string Name { get; set; }

        public List<Product>? Products { get; set; }
    }
}

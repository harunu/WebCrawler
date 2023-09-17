using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WebCrawler.Data.Models
{
    public class PropertyListing
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string? Title { get; set; }

        [Required]
        public decimal Price { get; set; }

        public string? Address { get; set; }

        public string? AdditionalCosts { get; set; }

        public string? Contact { get; set; }

        public string? ImageUrl { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        // New fields for address components
        public string? Street { get; set; }
        public string? StreetNumber { get; set; }
        public string? ZipCode { get; set; }
        public string? City { get; set; }
    }
}

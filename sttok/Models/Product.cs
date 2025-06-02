namespace sttok.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Product
    {
        [Key]
        public int UrunId { get; set; }

        [Required]
        [StringLength(50)]
        public string? UrunBarkod { get; set; }

        [Required]
        [StringLength(100)]
        public string? UrunAd { get; set; }

        [Required]
        [StringLength(50)]
        public string? UrunCesit { get; set; }

        [Required]
        public decimal UrunEn { get; set; }

        [Required]
        public decimal UrunBoy { get; set; }

        [Required]
        [StringLength(50)]
        public string? UrunDesenKod { get; set; }

        [Required]
        [StringLength(50)]
        public string? UrunRenk { get; set; }

        
        
    }
} 
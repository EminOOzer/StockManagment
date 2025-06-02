using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sttok.Models
{
    public class DepoStok
    {
        [Key]
        public int StokId { get; set; }

        [Required]
        public int UrunId { get; set; }

        [ForeignKey("UrunId")]
        public Product? Product { get; set; }

        [Required]
        [Display(Name = "Stok Adedi")]
        public int StokAdedi { get; set; }

        [NotMapped]
        [Display(Name = "Toplam Alan (m²)")]
        public decimal ToplamAlan => (Product?.UrunEn ?? 0) * (Product?.UrunBoy ?? 0) * StokAdedi;

        // Ürün bilgilerini kolayca erişmek için navigation property'ler
        [NotMapped]
        public string? UrunBarkod => Product?.UrunBarkod;
        
        [NotMapped]
        public string? UrunAd => Product?.UrunAd;
        
        [NotMapped]
        public string? UrunCesit => Product?.UrunCesit;
        
        [NotMapped]
        public decimal UrunEn => Product?.UrunEn ?? 0;
        
        [NotMapped]
        public decimal UrunBoy => Product?.UrunBoy ?? 0;
        
        [NotMapped]
        public string? UrunDesenKod => Product?.UrunDesenKod;
        
        [NotMapped]
        public string? UrunRenk => Product?.UrunRenk;
    }
} 
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sttok.Models
{
    public class StokHareket
    {
        [Key]
        public int HareketId { get; set; }

        [Required]
        public int UrunId { get; set; }

        [ForeignKey("UrunId")]
        public Product? Product { get; set; }

        [Required]
        public int Miktar { get; set; }

        [Required]
        public string HareketTipi { get; set; } = string.Empty; // "Giriş" veya "Çıkış"

        [Required]
        public DateTime IslemTarihi { get; set; } = DateTime.Now;

        [Required]
        public string KullaniciAdi { get; set; } = string.Empty;

        [NotMapped]
        public string? UrunBarkod => Product?.UrunBarkod;
        
        [NotMapped]
        public string? UrunAd => Product?.UrunAd;
        
        [NotMapped]
        public string? UrunCesit => Product?.UrunCesit;
    }
} 
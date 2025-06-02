using System.Collections.Generic;

namespace sttok.Models
{
    public class GunlukIslemlerViewModel
    {
        public List<StokHareket>? Hareketler { get; set; }
        public string? SecilenTarih { get; set; }
        public int ToplamSayfa { get; set; }
        public int SuankiSayfa { get; set; }
        public int SayfaBoyutu { get; set; }
        public int ToplamKayit { get; set; }
    }
} 
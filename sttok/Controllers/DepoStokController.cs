using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sttok.Data;
using sttok.Models;

namespace sttok.Controllers
{
    public class DepoStokController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DepoStokController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var stoklar = await _context.DepoStok
                .Include(d => d.Product)
                .ToListAsync();
            return View(stoklar);
        }

        // GET: DepoStok/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Products = await _context.Products.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(DepoStok depoStok)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Önce ürünü bul
                    var urun = await _context.Products.FindAsync(depoStok.UrunId);
                    if (urun == null)
                    {
                        return Json(new { success = false, message = "Ürün bulunamadı!" });
                    }

                    // Mevcut stok kaydını kontrol et
                    var mevcutStok = await _context.DepoStok
                        .FirstOrDefaultAsync(x => x.UrunId == depoStok.UrunId);

                    if (mevcutStok != null)
                    {
                        // Mevcut stok varsa güncelle
                        mevcutStok.StokAdedi += depoStok.StokAdedi;
                        _context.Update(mevcutStok);
                    }
                    else
                    {
                        // Yeni stok kaydı oluştur
                        _context.Add(depoStok);
                    }

                    // Stok hareketi kaydet
                    var stokHareket = new StokHareket
                    {
                        UrunId = depoStok.UrunId,
                        Miktar = depoStok.StokAdedi,
                        HareketTipi = "Giriş",
                        IslemTarihi = DateTime.Now,
                        KullaniciAdi = User.Identity?.Name ?? "Sistem"
                    };
                    _context.StokHareketler.Add(stokHareket);

                    await _context.SaveChangesAsync();
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "Geçersiz veri!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Stok girişi yapılırken bir hata oluştu: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, int stokAdedi)
        {
            var stok = await _context.DepoStok.FindAsync(id);
            if (stok == null)
            {
                return NotFound();
            }

            stok.StokAdedi = stokAdedi;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var stok = await _context.DepoStok.FindAsync(id);
            if (stok == null)
            {
                return NotFound();
            }

            _context.DepoStok.Remove(stok);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> GetProducts()
        {
            var products = await _context.Products.ToListAsync();
            return Json(products);
        }

        [HttpGet]
        public async Task<IActionResult> GetRapor()
        {
            try
            {
                var stokHareketleri = await _context.DepoStok
                    .Include(d => d.Product)
                    .OrderByDescending(s => s.StokId)
                    .Select(s => new
                    {
                        urunAd = s.Product == null ? "Bilinmeyen Ürün" : s.Product.UrunAd,
                        miktar = s.StokAdedi,
                        kullanici = "Sistem"
                    })
                    .ToListAsync();

                return Json(new { success = true, data = stokHareketleri });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Rapor alınırken bir hata oluştu." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> StokDus(int id, int miktar)
        {
            var stok = await _context.DepoStok.FindAsync(id);
            if (stok == null)
            {
                return Json(new { success = false, message = "Stok bulunamadı." });
            }

            if (miktar <= 0)
            {
                return Json(new { success = false, message = "Geçersiz miktar." });
            }

            if (stok.StokAdedi < miktar)
            {
                return Json(new { success = false, message = "Stokta yeterli ürün bulunmamaktadır." });
            }

            stok.StokAdedi -= miktar;
            
            // Stok hareketi kaydet
            var stokHareket = new StokHareket
            {
                UrunId = stok.UrunId,
                Miktar = miktar,
                HareketTipi = "Çıkış",
                IslemTarihi = DateTime.Now,
                KullaniciAdi = User.Identity?.Name ?? "Sistem"
            };
            _context.StokHareketler.Add(stokHareket);
            
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Stok başarıyla düşürüldü." });
        }

        [HttpGet]
        [Route("DepoStok/GunlukIslemler")]
        public async Task<IActionResult> GunlukIslemler(DateTime? tarih, int sayfa = 1, int sayfaBoyutu = 10)
        {
            try
            {
                var secilenTarih = tarih ?? DateTime.Today;
                var baslangic = secilenTarih.Date;
                var bitis = baslangic.AddDays(1);

                var toplamKayit = await _context.StokHareketler
                    .Where(h => h.IslemTarihi >= baslangic && h.IslemTarihi < bitis)
                    .CountAsync();

                var hareketler = await _context.StokHareketler
                    .Include(h => h.Product)
                    .Where(h => h.IslemTarihi >= baslangic && h.IslemTarihi < bitis)
                    .OrderByDescending(h => h.IslemTarihi)
                    .Skip((sayfa - 1) * sayfaBoyutu)
                    .Take(sayfaBoyutu)
                    .ToListAsync();

                var viewModel = new GunlukIslemlerViewModel
                {
                    Hareketler = hareketler,
                    SecilenTarih = secilenTarih.ToString("yyyy-MM-dd"),
                    ToplamSayfa = (int)Math.Ceiling(toplamKayit / (double)sayfaBoyutu),
                    SuankiSayfa = sayfa,
                    SayfaBoyutu = sayfaBoyutu,
                    ToplamKayit = toplamKayit
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                // Hata logla
                Console.WriteLine($"Günlük İşlemler Hatası: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                
                // Boş liste ile view'a dön
                return View(new List<StokHareket>());
            }
        }
    }
} 
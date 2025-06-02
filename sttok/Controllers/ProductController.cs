    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using sttok.Data;
    using sttok.Models;

    namespace sttok.Controllers
    {
        public class ProductController : Controller
        {
            private readonly ApplicationDbContext _context;

            public ProductController(ApplicationDbContext context)
            {
                _context = context;
            }

            // GET: Product
            public async Task<IActionResult> Index()
            {
                return View(await _context.Products.ToListAsync());
            }

            // GET: Product/Create
            public IActionResult Create()
            {
                return View();
            }

            // POST: Product/Create
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create([Bind("UrunBarkod,UrunAd,UrunCesit,UrunEn,UrunBoy,UrunDesenKod,UrunRenk")] Product product)
            {
                if (ModelState.IsValid)
                {
                    _context.Add(product);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(product);
            }

            // GET: Product/Edit/5
            public async Task<IActionResult> Edit(int? id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    return NotFound();
                }
                return Json(product); // Modal için JSON dönüyoruz
            }

            // POST: Product/Edit/5
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(int id, Product product)
            {
                if (id != product.UrunId)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(product);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!ProductExists(product.UrunId))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
                return View(product);
            }

            // POST: Product/Delete/5
            [HttpPost]
            public async Task<IActionResult> Delete(int id)
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    return Json(new { success = false, message = "Ürün bulunamadı." });
                }

                try
                {
                    _context.Products.Remove(product);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true });
                }
                catch (Exception)
                {
                    return Json(new { success = false, message = "Silme işlemi sırasında bir hata oluştu." });
                }
            }

            private bool ProductExists(int id)
            {
                return _context.Products.Any(e => e.UrunId == id);
            }
        }
    } 
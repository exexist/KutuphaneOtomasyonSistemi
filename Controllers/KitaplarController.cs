using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KutuphaneOtomasyon.Data;
using KutuphaneOtomasyon.Models;

namespace KutuphaneOtomasyon.Controllers
{
    [Authorize(Roles = "Admin")]
    public class KitaplarController : Controller
    {
        private readonly AppDbContext _context;

        public KitaplarController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? arama)
        {
            var sorgu = _context.Kitaplar.Include(k => k.Kategori).AsQueryable();

            if (!string.IsNullOrWhiteSpace(arama))
            {
                sorgu = sorgu.Where(k => k.Ad.Contains(arama) || k.Yazar.Contains(arama));
            }

            ViewBag.Arama = arama;
            return View(await sorgu.OrderBy(k => k.Ad).ToListAsync());
        }

        private async Task KategorileriYukle(int? seciliId = null)
        {
            ViewBag.KategoriId = new SelectList(
                await _context.Kategoriler.OrderBy(k => k.Ad).ToListAsync(), "Id", "Ad", seciliId);
        }

        public async Task<IActionResult> Ekle()
        {
            await KategorileriYukle();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ekle(Kitap kitap)
        {
            if (!ModelState.IsValid)
            {
                await KategorileriYukle(kitap.KategoriId);
                return View(kitap);
            }

            _context.Kitaplar.Add(kitap);
            await _context.SaveChangesAsync();
            TempData["Basari"] = "Kitap başarıyla eklendi.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Duzenle(int id)
        {
            var kitap = await _context.Kitaplar.FindAsync(id);
            if (kitap == null) return NotFound();
            await KategorileriYukle(kitap.KategoriId);
            return View(kitap);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Duzenle(int id, Kitap kitap)
        {
            if (id != kitap.Id) return NotFound();
            if (!ModelState.IsValid)
            {
                await KategorileriYukle(kitap.KategoriId);
                return View(kitap);
            }

            _context.Update(kitap);
            await _context.SaveChangesAsync();
            TempData["Basari"] = "Kitap başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Sil(int id)
        {
            var kitap = await _context.Kitaplar.Include(k => k.Kategori).FirstOrDefaultAsync(k => k.Id == id);
            if (kitap == null) return NotFound();
            return View(kitap);
        }

        [HttpPost, ActionName("Sil")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SilOnay(int id)
        {
            var kitap = await _context.Kitaplar.FindAsync(id);
            if (kitap != null)
            {
                var kullanimda = await _context.OduncKayitlari.AnyAsync(o => o.KitapId == id);
                if (kullanimda)
                {
                    TempData["Hata"] = "Bu kitaba ait ödünç kayıtları olduğu için silinemez.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Kitaplar.Remove(kitap);
                await _context.SaveChangesAsync();
                TempData["Basari"] = "Kitap silindi.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

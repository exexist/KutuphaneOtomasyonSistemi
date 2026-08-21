using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KutuphaneOtomasyon.Data;
using KutuphaneOtomasyon.Models;

namespace KutuphaneOtomasyon.Controllers
{
    [Authorize(Roles = "Admin")]
    public class KategorilerController : Controller
    {
        private readonly AppDbContext _context;

        public KategorilerController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Kategoriler.OrderBy(k => k.Ad).ToListAsync());
        }

        public IActionResult Ekle()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ekle(Kategori kategori)
        {
            if (!ModelState.IsValid)
            {
                return View(kategori);
            }

            _context.Kategoriler.Add(kategori);
            await _context.SaveChangesAsync();
            TempData["Basari"] = "Kategori başarıyla eklendi.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Duzenle(int id)
        {
            var kategori = await _context.Kategoriler.FindAsync(id);
            if (kategori == null) return NotFound();
            return View(kategori);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Duzenle(int id, Kategori kategori)
        {
            if (id != kategori.Id) return NotFound();
            if (!ModelState.IsValid)
            {
                return View(kategori);
            }

            _context.Update(kategori);
            await _context.SaveChangesAsync();
            TempData["Basari"] = "Kategori başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Sil(int id)
        {
            var kategori = await _context.Kategoriler.FindAsync(id);
            if (kategori == null) return NotFound();
            return View(kategori);
        }

        [HttpPost, ActionName("Sil")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SilOnay(int id)
        {
            var kategori = await _context.Kategoriler.FindAsync(id);
            if (kategori != null)
            {
                var kullanimda = await _context.Kitaplar.AnyAsync(k => k.KategoriId == id);
                if (kullanimda)
                {
                    TempData["Hata"] = "Bu kategoriye ait kitaplar olduğu için silinemez.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Kategoriler.Remove(kategori);
                await _context.SaveChangesAsync();
                TempData["Basari"] = "Kategori silindi.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

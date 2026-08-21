using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KutuphaneOtomasyon.Data;
using KutuphaneOtomasyon.Models;

namespace KutuphaneOtomasyon.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OduncController : Controller
    {
        private readonly AppDbContext _context;

        public OduncController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var kayitlar = await _context.OduncKayitlari
                .Include(o => o.Kitap)
                .Include(o => o.Uye)
                .OrderByDescending(o => o.AlisTarihi)
                .ToListAsync();
            return View(kayitlar);
        }

        private async Task ListeleriYukle()
        {
            ViewBag.KitapId = new SelectList(
                await _context.Kitaplar.Where(k => k.StokAdedi > 0).OrderBy(k => k.Ad).ToListAsync(), "Id", "Ad");
            ViewBag.UyeId = new SelectList(
                await _context.Uyeler.OrderBy(u => u.AdSoyad).ToListAsync(), "Id", "AdSoyad");
        }

        public async Task<IActionResult> Ekle()
        {
            await ListeleriYukle();
            return View(new Odunc());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ekle(Odunc odunc)
        {
            var kitap = await _context.Kitaplar.FindAsync(odunc.KitapId);
            if (kitap == null || kitap.MusaitAdet <= 0)
            {
                ModelState.AddModelError(string.Empty, "Seçilen kitap için müsait stok bulunmuyor.");
            }

            if (!ModelState.IsValid)
            {
                await ListeleriYukle();
                return View(odunc);
            }

            _context.OduncKayitlari.Add(odunc);
            await _context.SaveChangesAsync();
            TempData["Basari"] = "Ödünç kaydı oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IadeAl(int id)
        {
            var odunc = await _context.OduncKayitlari.FindAsync(id);
            if (odunc != null && !odunc.IadeEdildiMi)
            {
                odunc.IadeEdildiMi = true;
                odunc.IadeTarihi = DateTime.Now;
                await _context.SaveChangesAsync();
                TempData["Basari"] = "Kitap iadesi alındı.";
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Sil(int id)
        {
            var odunc = await _context.OduncKayitlari
                .Include(o => o.Kitap)
                .Include(o => o.Uye)
                .FirstOrDefaultAsync(o => o.Id == id);
            if (odunc == null) return NotFound();
            return View(odunc);
        }

        [HttpPost, ActionName("Sil")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SilOnay(int id)
        {
            var odunc = await _context.OduncKayitlari.FindAsync(id);
            if (odunc != null)
            {
                _context.OduncKayitlari.Remove(odunc);
                await _context.SaveChangesAsync();
                TempData["Basari"] = "Ödünç kaydı silindi.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

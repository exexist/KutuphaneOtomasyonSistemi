using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KutuphaneOtomasyon.Data;
using KutuphaneOtomasyon.Models;

namespace KutuphaneOtomasyon.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UyelerController : Controller
    {
        private readonly AppDbContext _context;

        public UyelerController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? arama)
        {
            var sorgu = _context.Uyeler.AsQueryable();
            if (!string.IsNullOrWhiteSpace(arama))
            {
                sorgu = sorgu.Where(u => u.AdSoyad.Contains(arama) || u.Eposta.Contains(arama));
            }

            ViewBag.Arama = arama;
            return View(await sorgu.OrderBy(u => u.AdSoyad).ToListAsync());
        }

        public IActionResult Ekle() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ekle(Uye uye, string sifre)
        {
            if (string.IsNullOrWhiteSpace(sifre) || sifre.Length < 6)
            {
                ModelState.AddModelError("sifre", "Şifre en az 6 karakter olmalıdır.");
            }
            else
            {
                var epostaKullanimda = await _context.Uyeler
                    .AnyAsync(u => u.Eposta.ToLower() == uye.Eposta.ToLower());
                if (epostaKullanimda)
                {
                    ModelState.AddModelError(nameof(uye.Eposta), "Bu e-posta adresi ile zaten kayıtlı bir üye bulunuyor.");
                }
            }

            if (!ModelState.IsValid) return View(uye);

            uye.SifreHash = SifreYardimcisi.Hashle(sifre);
            _context.Uyeler.Add(uye);
            await _context.SaveChangesAsync();
            TempData["Basari"] = "Üye başarıyla eklendi.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Duzenle(int id)
        {
            var uye = await _context.Uyeler.FindAsync(id);
            if (uye == null) return NotFound();
            return View(uye);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Duzenle(int id, Uye uye, string? yeniSifre)
        {
            if (id != uye.Id) return NotFound();
            if (!string.IsNullOrWhiteSpace(yeniSifre) && yeniSifre.Length < 6)
            {
                ModelState.AddModelError("yeniSifre", "Şifre en az 6 karakter olmalıdır.");
            }

            if (!ModelState.IsValid) return View(uye);

            var mevcutUye = await _context.Uyeler.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
            if (mevcutUye == null) return NotFound();

            uye.SifreHash = string.IsNullOrWhiteSpace(yeniSifre)
                ? mevcutUye.SifreHash
                : SifreYardimcisi.Hashle(yeniSifre);

            _context.Update(uye);
            await _context.SaveChangesAsync();
            TempData["Basari"] = "Üye bilgileri güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Sil(int id)
        {
            var uye = await _context.Uyeler.FindAsync(id);
            if (uye == null) return NotFound();
            return View(uye);
        }

        [HttpPost, ActionName("Sil")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SilOnay(int id)
        {
            var uye = await _context.Uyeler.FindAsync(id);
            if (uye != null)
            {
                var kullanimda = await _context.OduncKayitlari.AnyAsync(o => o.UyeId == id);
                if (kullanimda)
                {
                    TempData["Hata"] = "Bu üyeye ait ödünç kayıtları olduğu için silinemez.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Uyeler.Remove(uye);
                await _context.SaveChangesAsync();
                TempData["Basari"] = "Üye silindi.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KutuphaneOtomasyon.Data;
using KutuphaneOtomasyon.Models;

namespace KutuphaneOtomasyon.Controllers
{
    public class UyeHesapController : Controller
    {
        private readonly AppDbContext _context;

        public UyeHesapController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Kayit()
        {
            return View(new UyeKayitViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Kayit(UyeKayitViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var epostaKullanimda = await _context.Uyeler
                .AnyAsync(u => u.Eposta.ToLower() == model.Eposta.ToLower());

            if (epostaKullanimda)
            {
                ModelState.AddModelError(nameof(model.Eposta), "Bu e-posta adresi ile zaten kayıtlı bir üye bulunuyor.");
                return View(model);
            }

            var yeniUye = new Uye
            {
                AdSoyad = model.AdSoyad.Trim(),
                Eposta = model.Eposta.Trim(),
                Telefon = model.Telefon,
                SifreHash = SifreYardimcisi.Hashle(model.Sifre),
                KayitTarihi = DateTime.Now
            };

            _context.Uyeler.Add(yeniUye);
            await _context.SaveChangesAsync();

            await GirisYap(yeniUye);

            TempData["Basari"] = "Kaydınız başarıyla oluşturuldu. Hoş geldiniz!";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Giris(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new UyeGirisViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Giris(UyeGirisViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var uye = await _context.Uyeler
                .FirstOrDefaultAsync(u => u.Eposta.ToLower() == model.Eposta.ToLower());

            if (uye == null || !SifreYardimcisi.Dogrula(model.Sifre, uye.SifreHash))
            {
                ModelState.AddModelError(string.Empty, "E-posta veya şifre hatalı.");
                return View(model);
            }

            await GirisYap(uye);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        private async Task GirisYap(Uye uye)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, uye.Eposta),
                new(ClaimTypes.NameIdentifier, uye.Id.ToString()),
                new(ClaimTypes.Role, "Uye"),
                new("AdSoyad", uye.AdSoyad)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cikis()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Uye")]
        public async Task<IActionResult> Hesabim()
        {
            var uyeId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var uye = await _context.Uyeler.FindAsync(uyeId);
            if (uye == null) return NotFound();

            var oduncKayitlari = await _context.OduncKayitlari
                .Include(o => o.Kitap)
                .Where(o => o.UyeId == uyeId)
                .OrderByDescending(o => o.AlisTarihi)
                .ToListAsync();

            ViewBag.OduncKayitlari = oduncKayitlari;
            return View(uye);
        }
    }
}

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KutuphaneOtomasyon.Data;
using KutuphaneOtomasyon.Models;

namespace KutuphaneOtomasyon.Controllers
{
    public class HesapController : Controller
    {
        private readonly AppDbContext _context;

        public HesapController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Giris(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new GirisViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Giris(GirisViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var kullanici = await _context.AdminKullanicilar
                .FirstOrDefaultAsync(a => a.KullaniciAdi == model.KullaniciAdi);

            if (kullanici == null || !SifreYardimcisi.Dogrula(model.Sifre, kullanici.SifreHash))
            {
                ModelState.AddModelError(string.Empty, "Kullanıcı adı veya şifre hatalı.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, kullanici.KullaniciAdi),
                new(ClaimTypes.NameIdentifier, kullanici.Id.ToString()),
                new(ClaimTypes.Role, "Admin"),
                new("AdSoyad", kullanici.AdSoyad ?? kullanici.KullaniciAdi)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Admin");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cikis()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ErisimEngellendi()
        {
            return View();
        }
    }
}

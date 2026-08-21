using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KutuphaneOtomasyon.Data;

namespace KutuphaneOtomasyon.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.ToplamKitap = await _context.Kitaplar.CountAsync();
            ViewBag.ToplamKategori = await _context.Kategoriler.CountAsync();
            ViewBag.ToplamUye = await _context.Uyeler.CountAsync();
            ViewBag.AktifOduncSayisi = await _context.OduncKayitlari.CountAsync(o => !o.IadeEdildiMi);
            ViewBag.GecikenOduncSayisi = await _context.OduncKayitlari
                .CountAsync(o => !o.IadeEdildiMi && o.TeslimTarihi < DateTime.Now);

            return View();
        }
    }
}

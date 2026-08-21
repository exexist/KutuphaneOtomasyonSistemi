using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KutuphaneOtomasyon.Data;

namespace KutuphaneOtomasyon.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var kitaplar = await _context.Kitaplar
                .Include(k => k.Kategori)
                .OrderBy(k => k.Ad)
                .ToListAsync();

            ViewBag.ToplamKitap = kitaplar.Count;
            ViewBag.ToplamKategori = await _context.Kategoriler.CountAsync();
            ViewBag.ToplamUye = await _context.Uyeler.CountAsync();

            return View(kitaplar);
        }

        public IActionResult Hata()
        {
            return View();
        }
    }
}

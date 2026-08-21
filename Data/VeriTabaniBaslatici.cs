using KutuphaneOtomasyon.Models;

namespace KutuphaneOtomasyon.Data
{
    public static class VeriTabaniBaslatici
    {
        public static void Baslat(AppDbContext context)
        {
            context.Database.EnsureCreated();

            if (!context.AdminKullanicilar.Any())
            {
                context.AdminKullanicilar.Add(new AdminKullanici
                {
                    KullaniciAdi = "admin",
                    SifreHash = SifreYardimcisi.Hashle("tekpark"),
                    AdSoyad = "Sistem Yöneticisi"
                });
                context.SaveChanges();
            }

            if (!context.Kategoriler.Any())
            {
                var kategoriler = new List<Kategori>
                {
                    new() { Ad = "Roman" },
                    new() { Ad = "Bilim Kurgu" },
                    new() { Ad = "Tarih" },
                    new() { Ad = "Kişisel Gelişim" },
                    new() { Ad = "Bilgisayar Bilimi" }
                };
                context.Kategoriler.AddRange(kategoriler);
                context.SaveChanges();

                var kitaplar = new List<Kitap>
                {
                    new() { Ad = "Simyacı", Yazar = "Paulo Coelho", YayinYili = 1988, KategoriId = kategoriler[0].Id, StokAdedi = 5 },
                    new() { Ad = "Dune", Yazar = "Frank Herbert", YayinYili = 1965, KategoriId = kategoriler[1].Id, StokAdedi = 3 },
                    new() { Ad = "Nutuk", Yazar = "Mustafa Kemal Atatürk", YayinYili = 1927, KategoriId = kategoriler[2].Id, StokAdedi = 4 },
                    new() { Ad = "Atomik Alışkanlıklar", Yazar = "James Clear", YayinYili = 2018, KategoriId = kategoriler[3].Id, StokAdedi = 6 },
                    new() { Ad = "Temiz Kod", Yazar = "Robert C. Martin", YayinYili = 2008, KategoriId = kategoriler[4].Id, StokAdedi = 2 }
                };
                context.Kitaplar.AddRange(kitaplar);
                context.SaveChanges();
            }

            if (!context.Uyeler.Any())
            {
                var ornekSifreHash = SifreYardimcisi.Hashle("Uye123!");
                context.Uyeler.AddRange(
                    new Uye { AdSoyad = "Ahmet Yılmaz", Eposta = "ahmet.yilmaz@ornek.com", Telefon = "05551112233", SifreHash = ornekSifreHash },
                    new Uye { AdSoyad = "Elif Kaya", Eposta = "elif.kaya@ornek.com", Telefon = "05552223344", SifreHash = ornekSifreHash }
                );
                context.SaveChanges();
            }
        }
    }
}

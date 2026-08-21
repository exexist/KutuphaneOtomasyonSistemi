# 📚 Kütüphane Otomasyon Sistemi

ASP.NET Core MVC (.NET 10) + Entity Framework Core + SQLite ile geliştirilmiş,
üyelik sistemi ve admin panelli bir kütüphane yönetim sistemi.

## Özellikler
- **Herkese açık anasayfa**: kitap kataloğu listesi
- **Üye sistemi**: ziyaretçiler "Üye Ol" ile kayıt olabilir, "Üye Girişi" ile
  giriş yapabilir, "Hesabım" sayfasından kendi ödünç geçmişini görebilir
- **Admin paneli**: ayrı kullanıcı adı/şifre ile giriş yapan yöneticiler için
  Kitap, Kategori, Üye ve Ödünç işlemleri (Ekle/Düzenle/Sil)
- Rol tabanlı yetkilendirme: Admin sayfaları sadece "Admin" rolüne,
  Hesabım sayfası sadece "Uye" rolüne açıktır
- Ödünç verme / iade alma işlemleri, gecikmiş kitapların uyarı ile gösterimi
- Şifreler SHA-256 + salt ile hashlenir, düz metin hiçbir yerde tutulmaz
- E-posta adresi benzersizdir (aynı e-posta ile iki üye kaydı oluşturulamaz)
- SQLite veritabanı — ilk çalıştırmada otomatik oluşur ve örnek verilerle doldurulur
- Tamamen Türkçe arayüz, profesyonel/modern Bootstrap 5 tabanlı özel tasarım
  (özel renk paleti, Google Fonts, hover efektleri, gradient hero bölümü)

## Giriş Bilgileri

**Admin girişi** (/Hesap/Giris):
- Kullanıcı adı: **admin**
- Şifre: **tekpark**

**Örnek üye girişi** (/UyeHesap/Giris) — test amaçlı, seed veriyle gelir:
- E-posta: **ahmet.yilmaz@ornek.com**
- Şifre: **Uye123!**

Yeni üyeler anasayfadaki **"Üye Ol"** butonundan kendi hesaplarını
oluşturabilir.

## ⚠️ Önemli — Daha Önce Çalıştırdıysanız
Projeyi daha önceki bir sürümüyle çalıştırıp `kutuphane.db` dosyası zaten
oluştuysa, yeni üyelik alanları (şifre vb.) eski veritabanında bulunmadığı
için hata alabilirsiniz. Bu durumda proje klasöründeki (bin/Debug/net10.0
altındaki veya proje kök dizinindeki) **kutuphane.db** dosyasını silip
uygulamayı tekrar çalıştırın; veritabanı yeni şemayla ve yeni admin
bilgileriyle (`admin` / `tekpark`) otomatik olarak yeniden oluşturulacaktır.

## Visual Studio'da Çalıştırma
1. `KutuphaneOtomasyon.csproj` dosyasını Visual Studio 2026 ile açın
   (.NET 10 SDK kurulu olmalı).
2. Visual Studio ilk açılışta NuGet paketlerini otomatik geri yükleyecektir.
   Sürüm uyarısı alırsanız, projeye sağ tıklayıp **NuGet Paketlerini Yönet**
   menüsünden paketleri güncel sürüme yükseltmeniz yeterlidir.
3. `F5` veya `Ctrl+F5` ile projeyi çalıştırın.
4. Uygulama ilk açılışta `kutuphane.db` dosyasını oluşturacak, örnek
   kitap/kategori/üye verileriyle ve varsayılan admin hesabıyla dolduracaktır.

## Proje Yapısı
```
KutuphaneOtomasyon/
├── Controllers/
│   ├── Home, Kitaplar, Kategoriler, Uyeler, Odunc   (genel + admin işlemleri)
│   ├── Hesap            -> Admin giriş/çıkış
│   └── UyeHesap          -> Üye kayıt/giriş/çıkış/Hesabım
├── Models/                Kitap, Kategori, Uye, Odunc, AdminKullanici,
│                           GirisViewModel, UyeKayitViewModel, UyeGirisViewModel
├── Data/                  AppDbContext, SifreYardimcisi (hash), VeriTabaniBaslatici (seed)
├── Views/                 Razor görünümleri (Türkçe arayüz)
├── wwwroot/css/           Özel, profesyonel site stili
└── Program.cs             DB, tek şema Cookie Auth (Rol: Admin / Uye), Routing
```

## Notlar
- Admin ve üye girişleri aynı çerez (cookie) şemasını kullanır; kim olduğu
  "Role" claim'i ile ayırt edilir (`Admin` / `Uye`). Bu nedenle aynı tarayıcıda
  aynı anda hem admin hem üye olarak giriş yapılamaz — biri diğerinin
  oturumunun yerini alır (küçük ölçekli bir proje için tasarım tercihidir).
- Admin panelinden üye eklerken/düzenlerken şifre alanı admin tarafından
  belirlenir; üye kendi şifresini "Üye Ol" sayfasından belirler.
- Kategorisi/ödünç kaydı olan kitaplar ve üyeler silinmeye çalışıldığında
  sistem uyarı vererek veri bütünlüğünü korur.

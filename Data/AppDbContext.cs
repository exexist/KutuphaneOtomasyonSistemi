using Microsoft.EntityFrameworkCore;
using KutuphaneOtomasyon.Models;

namespace KutuphaneOtomasyon.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Kategori> Kategoriler => Set<Kategori>();
        public DbSet<Kitap> Kitaplar => Set<Kitap>();
        public DbSet<Uye> Uyeler => Set<Uye>();
        public DbSet<Odunc> OduncKayitlari => Set<Odunc>();
        public DbSet<AdminKullanici> AdminKullanicilar => Set<AdminKullanici>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Odunc>()
                .HasOne(o => o.Kitap)
                .WithMany(k => k.OduncKayitlari)
                .HasForeignKey(o => o.KitapId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Odunc>()
                .HasOne(o => o.Uye)
                .WithMany(u => u.OduncKayitlari)
                .HasForeignKey(o => o.UyeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Kitap>()
                .HasOne(k => k.Kategori)
                .WithMany(kt => kt.Kitaplar)
                .HasForeignKey(k => k.KategoriId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AdminKullanici>()
                .HasIndex(a => a.KullaniciAdi)
                .IsUnique();

            modelBuilder.Entity<Uye>()
                .HasIndex(u => u.Eposta)
                .IsUnique();
        }
    }
}

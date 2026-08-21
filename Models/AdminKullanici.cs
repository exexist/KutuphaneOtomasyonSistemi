using System.ComponentModel.DataAnnotations;

namespace KutuphaneOtomasyon.Models
{
    public class AdminKullanici
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Kullanıcı Adı")]
        public string KullaniciAdi { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Şifre (Hash)")]
        public string SifreHash { get; set; } = string.Empty;

        [Display(Name = "Ad Soyad")]
        [StringLength(150)]
        public string? AdSoyad { get; set; }
    }
}

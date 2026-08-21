using System.ComponentModel.DataAnnotations;

namespace KutuphaneOtomasyon.Models
{
    public class UyeKayitViewModel
    {
        [Required(ErrorMessage = "Ad Soyad zorunludur.")]
        [StringLength(150)]
        [Display(Name = "Ad Soyad")]
        public string AdSoyad { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [Display(Name = "E-posta")]
        public string Eposta { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        [Display(Name = "Telefon (isteğe bağlı)")]
        public string? Telefon { get; set; }

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Sifre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre tekrarı zorunludur.")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre (Tekrar)")]
        [Compare(nameof(Sifre), ErrorMessage = "Şifreler birbiriyle eşleşmiyor.")]
        public string SifreTekrar { get; set; } = string.Empty;
    }
}

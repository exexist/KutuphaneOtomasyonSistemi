using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace KutuphaneOtomasyon.Models
{
    public class Uye
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad Soyad zorunludur.")]
        [StringLength(150)]
        [Display(Name = "Ad Soyad")]
        public string AdSoyad { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [Display(Name = "E-posta")]
        public string Eposta { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        [Display(Name = "Telefon")]
        public string? Telefon { get; set; }

        [ValidateNever]
        [Display(Name = "Şifre")]
        public string SifreHash { get; set; } = string.Empty;

        [Display(Name = "Kayıt Tarihi")]
        [DataType(DataType.Date)]
        public DateTime KayitTarihi { get; set; } = DateTime.Now;

        public ICollection<Odunc>? OduncKayitlari { get; set; }
    }
}

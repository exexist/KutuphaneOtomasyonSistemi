using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KutuphaneOtomasyon.Models
{
    public class Kitap
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kitap adı zorunludur.")]
        [StringLength(200)]
        [Display(Name = "Kitap Adı")]
        public string Ad { get; set; } = string.Empty;

        [Required(ErrorMessage = "Yazar adı zorunludur.")]
        [StringLength(150)]
        [Display(Name = "Yazar")]
        public string Yazar { get; set; } = string.Empty;

        [StringLength(20)]
        [Display(Name = "ISBN")]
        public string? Isbn { get; set; }

        [Display(Name = "Yayın Yılı")]
        [Range(1000, 2100)]
        public int? YayinYili { get; set; }

        [Required]
        [Display(Name = "Kategori")]
        public int KategoriId { get; set; }

        [ForeignKey(nameof(KategoriId))]
        public Kategori? Kategori { get; set; }

        [Display(Name = "Stok Adedi")]
        [Range(0, 10000)]
        public int StokAdedi { get; set; } = 1;

        public ICollection<Odunc>? OduncKayitlari { get; set; }

        [NotMapped]
        public int MusaitAdet => StokAdedi - (OduncKayitlari?.Count(o => !o.IadeEdildiMi) ?? 0);
    }
}

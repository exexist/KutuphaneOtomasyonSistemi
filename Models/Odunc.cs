using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KutuphaneOtomasyon.Models
{
    public class Odunc
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Kitap")]
        public int KitapId { get; set; }

        [ForeignKey(nameof(KitapId))]
        public Kitap? Kitap { get; set; }

        [Required]
        [Display(Name = "Üye")]
        public int UyeId { get; set; }

        [ForeignKey(nameof(UyeId))]
        public Uye? Uye { get; set; }

        [Display(Name = "Alış Tarihi")]
        [DataType(DataType.Date)]
        public DateTime AlisTarihi { get; set; } = DateTime.Now;

        [Display(Name = "Teslim Tarihi")]
        [DataType(DataType.Date)]
        public DateTime TeslimTarihi { get; set; } = DateTime.Now.AddDays(14);

        [Display(Name = "İade Edildi mi?")]
        public bool IadeEdildiMi { get; set; } = false;

        [Display(Name = "İade Tarihi")]
        [DataType(DataType.Date)]
        public DateTime? IadeTarihi { get; set; }
    }
}

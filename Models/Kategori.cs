using System.ComponentModel.DataAnnotations;

namespace KutuphaneOtomasyon.Models
{
    public class Kategori
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kategori adı zorunludur.")]
        [StringLength(100)]
        [Display(Name = "Kategori Adı")]
        public string Ad { get; set; } = string.Empty;

        public ICollection<Kitap>? Kitaplar { get; set; }
    }
}

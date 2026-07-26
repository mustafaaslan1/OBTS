using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OBTS_Web.Models
{
    public class Ariza
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Başlık zorunludur.")]
        [StringLength(150)]
        public string Baslik { get; set; }

        [Required(ErrorMessage = "Açıklama zorunludur.")]
        public string Aciklama { get; set; }

        [Required(ErrorMessage = "BİM / Demirbaş kodu zorunludur.")]
        [StringLength(50)]
        public string BimKodu { get; set; }

        [Required(ErrorMessage = "Ürün marka/model bilgisi zorunludur.")]
        [StringLength(100)]
        public string UrunMarkaModel { get; set; }

        [Required(ErrorMessage = "Ad Soyad alanı zorunludur.")]
        [StringLength(100)]
        public string BildirenKisi { get; set; }

        [Required(ErrorMessage = "Oda numarası zorunludur.")]
        [StringLength(20)]
        public string OdaNo { get; set; }

        [Required(ErrorMessage = "Dahili numara zorunludur.")]
        [StringLength(10)]
        public string DahiliNo { get; set; }

        [Required(ErrorMessage = "Öncelik derecesi seçilmelidir.")]
        public string OncelikDerecesi { get; set; }

        [Required(ErrorMessage = "Arıza kategorisi seçilmelidir.")]
        public string Kategori { get; set; }
        public bool UcusOperasyonunuEtkiliyorMu { get; set; } 

        public string? FotografYolu { get; set; }

        [Required]
        public string Durum { get; set; } = "Tekniker Alınmayı Bekliyor";

        public DateTime BildirimTarihi { get; set; } = DateTime.Now;
        public DateTime? MudahaleBaslangicTarihi { get; set; }
        public DateTime? FirmaGonderimTarihi { get; set; }
        public DateTime? CozumTarihi { get; set; }

        public int KonumId { get; set; }

        [ForeignKey("KonumId")]
        public Konum? Konum { get; set; }

        public List<ArizaIslemGecmisi>? IslemGecmisleri { get; set; }

        public string? KaydedenKullaniciEmail { get; set; }
    }
}
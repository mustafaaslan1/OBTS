using System.ComponentModel.DataAnnotations;

namespace OBTS_Web.Models
{
    public class KullaniciDetayViewModel
    {
        public string UserId { get; set; } = null!;

        [Display(Name = "E-Posta Adresi")]
        public string? Email { get; set; }

        [Display(Name = "Ad Soyad")]
        [StringLength(100, ErrorMessage = "En fazla 100 karakter girilebilir.")]
        public string? AdSoyad { get; set; }

        [Display(Name = "Dahili Hat (Örn: 1234)")]
        [StringLength(10, ErrorMessage = "En fazla 10 karakter girilebilir.")]
        public string? DahiliHat { get; set; }

        [Display(Name = "Sistem Rolü")]
        public string? Rol { get; set; }

        [Display(Name = "Hesap Onaylı Mı?")]
        public bool IsApproved { get; set; }
    }
}
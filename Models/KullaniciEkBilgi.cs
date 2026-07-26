using System.ComponentModel.DataAnnotations;

namespace OBTS_Web.Models
{
    public class KullaniciEkBilgi
    {
        [Key]
        public string UserId { get; set; } = null!;

        [StringLength(100, ErrorMessage = "Ad ve Soyad en fazla 100 karakter olabilir.")]
        public string? AdSoyad { get; set; }

        [StringLength(10, ErrorMessage = "Dahili hat en fazla 10 karakter olabilir.")]
        public string? DahiliHat { get; set; } 

        public bool IsApproved { get; set; } = false;

        public bool IsTemporaryPassword { get; set; } = false;
    }
}
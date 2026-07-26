namespace OBTS_Web.Models
{
    public class KullaniciListeViewModel
    {
        public string UserId { get; set; } = null!;
        public string? Email { get; set; }
        public string? AdSoyad { get; set; }
        public string? DahiliHat { get; set; }
        public string? Rol { get; set; }
        public bool IsApproved { get; set; }
    }
}
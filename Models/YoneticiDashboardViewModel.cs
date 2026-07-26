using System.Collections.Generic;

namespace OBTS_Web.Models
{
    public class YoneticiDashboardViewModel
    {
        public int ToplamArizaSayisi { get; set; }
        public int AcilOncelikliArizaSayisi { get; set; }
        public int FirmadaBekleyenArizaSayisi { get; set; }
        public int CozulenArizaSayisi { get; set; }
        public int OnayBekleyenKullaniciSayisi { get; set; }

        public List<OnayBekleyenKullanici> OnayBekleyenKullanicilar { get; set; } = new List<OnayBekleyenKullanici>();

        public List<TeknikerPerformans> TeknikerPerformanslari { get; set; } = new List<TeknikerPerformans>();
    }

    public class OnayBekleyenKullanici
    {
        public string UserId { get; set; }
        public string Email { get; set; }
    }

    public class TeknikerPerformans
    {
        public string TeknikerAdSoyad { get; set; }
        public int ToplamMudahale { get; set; }
        public int CozulenIslem { get; set; }
    }
}
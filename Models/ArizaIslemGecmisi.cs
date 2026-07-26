using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OBTS_Web.Models
{
    public class ArizaIslemGecmisi
    {
        [Key]
        public int Id { get; set; }

        public int ArizaId { get; set; }

        [ForeignKey("ArizaId")]
        public Ariza? Ariza { get; set; }

        [Required]
        public string TeknikerAdSoyad { get; set; }

        [Required]
        public string IslemAciklamasi { get; set; }

        public DateTime IslemTarihi { get; set; } = DateTime.Now;
    }
}
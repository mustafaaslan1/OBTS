using System.ComponentModel.DataAnnotations;

namespace OBTS_Web.Models
{
    public class Konum
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Konum adı zorunludur.")]
        [StringLength(100)]
        public string Ad { get; set; }
    }
}
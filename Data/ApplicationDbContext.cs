using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OBTS_Web.Models;

namespace OBTS_Web.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Ariza> Arizalar { get; set; }
        public DbSet<Konum> Konumlar { get; set; }
        public DbSet<ArizaIslemGecmisi> ArizaIslemGecmisleri { get; set; }

        public DbSet<KullaniciEkBilgi> KullaniciEkBilgileri { get; set; }
    }
}
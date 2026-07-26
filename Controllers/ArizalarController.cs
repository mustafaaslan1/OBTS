using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using OBTS_Web.Data;
using OBTS_Web.Models;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace OBTS_Web.Controllers
{
    [Authorize]
    public class ArizalarController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ArizalarController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Create()
        {
            ViewBag.Konumlar = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Konumlar, "Id", "Ad");
            return View();
        }

        [Authorize(Roles = "Personel, Yonetici")]
        public IActionResult PersonelDashboard()
        {
            var aktifKullanici = User.Identity?.Name;

            var kullaniciArizalari = _context.Arizalar
                .Include(a => a.Konum)
                .Include(a => a.IslemGecmisleri)
                .Where(a => a.KaydedenKullaniciEmail == aktifKullanici)
                .OrderByDescending(a => a.Id)
                .ToList();

            return View(kullaniciArizalari);
        }

        [HttpPost]
        [Authorize(Roles = "Personel")]
        public async Task<IActionResult> KisiOnayiVer(int id)
        {
            var ariza = await _context.Arizalar.FindAsync(id);
            var aktifKullanici = User.Identity?.Name;

            if (ariza != null && ariza.Durum == "Kişi Onayında" && ariza.KaydedenKullaniciEmail == aktifKullanici)
            {
                ariza.Durum = "İşlem Bitti";
                if (!ariza.CozumTarihi.HasValue)
                {
                    ariza.CozumTarihi = DateTime.Now;
                }

                var onayIslemi = new ArizaIslemGecmisi
                {
                    ArizaId = id,
                    TeknikerAdSoyad = aktifKullanici, 
                    IslemAciklamasi = "Personel tarafından ürünün teslim alındığı ve sorunun giderildiği onaylandı.",
                    IslemTarihi = DateTime.Now
                };
                _context.ArizaIslemGecmisleri.Add(onayIslemi);

                await _context.SaveChangesAsync();
                TempData["BasariMesaji"] = "Arıza kaydı başarıyla kapatıldı ve onaylandı.";
            }

            return RedirectToAction(nameof(PersonelDashboard));
        }

        [Authorize(Roles = "Tekniker, Yonetici")]
        public IActionResult TeknikerDashboard()
        {
            var tumArizalar = _context.Arizalar
                .Include(a => a.Konum)
                .Include(a => a.IslemGecmisleri)
                .OrderByDescending(a => a.Id)
                .ToList();

            return View(tumArizalar);
        }

        [Authorize(Roles = "Yonetici")]
        public async Task<IActionResult> YoneticiDashboard()
        {
            var viewModel = new YoneticiDashboardViewModel();

            viewModel.ToplamArizaSayisi = await _context.Arizalar.CountAsync();

            viewModel.AcilOncelikliArizaSayisi = await _context.Arizalar.CountAsync(a => a.Durum != null && a.Durum.Contains("Acil"));

            viewModel.FirmadaBekleyenArizaSayisi = await _context.Arizalar.CountAsync(a => a.Durum == "Firmaya Gönderildi");
            viewModel.CozulenArizaSayisi = await _context.Arizalar.CountAsync(a => a.Durum == "İşlem Bitti");

            var onaysizKullanicilarDb = await _context.KullaniciEkBilgileri
                .Where(k => k.IsApproved == false)
                .ToListAsync();

            viewModel.OnayBekleyenKullaniciSayisi = onaysizKullanicilarDb.Count;

            foreach (var item in onaysizKullanicilarDb)
            {
                var user = await _userManager.FindByIdAsync(item.UserId);
                if (user != null)
                {
                    viewModel.OnayBekleyenKullanicilar.Add(new OnayBekleyenKullanici
                    {
                        UserId = user.Id,
                        Email = user.Email
                    });
                }
            }

            // LINQ GroupBy ile İşlem Geçmişinden Gruplama
            var performansData = await _context.ArizaIslemGecmisleri
                .GroupBy(g => g.TeknikerAdSoyad)
                .Select(g => new
                {
                    Tekniker = g.Key,
                    Toplam = g.Count()
                })
                .ToListAsync();

            foreach (var p in performansData)
            {
                var teknikerinArizaIdleri = await _context.ArizaIslemGecmisleri
                    .Where(x => x.TeknikerAdSoyad == p.Tekniker)
                    .Select(x => x.ArizaId)
                    .Distinct()
                    .ToListAsync();

                var cozulenler = await _context.Arizalar
                    .Where(a => teknikerinArizaIdleri.Contains(a.Id) && a.Durum == "İşlem Bitti")
                    .CountAsync();

                viewModel.TeknikerPerformanslari.Add(new TeknikerPerformans
                {
                    TeknikerAdSoyad = string.IsNullOrEmpty(p.Tekniker) ? "Bilinmeyen Tekniker" : p.Tekniker,
                    ToplamMudahale = p.Toplam,
                    CozulenIslem = cozulenler
                });
            }

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Tekniker, Yonetici")]
        public async Task<IActionResult> YeniIslemEkle(int ArizaId, string TeknikerAdSoyad, string YeniDurum, string IslemAciklamasi)
        {
            var ariza = await _context.Arizalar.FindAsync(ArizaId);
            if (ariza == null)
            {
                return NotFound();
            }

            var yeniIslem = new ArizaIslemGecmisi
            {
                ArizaId = ArizaId,
                TeknikerAdSoyad = TeknikerAdSoyad,
                IslemAciklamasi = IslemAciklamasi,
                IslemTarihi = DateTime.Now
            };

            _context.ArizaIslemGecmisleri.Add(yeniIslem);

            if (ariza.Durum != YeniDurum)
            {
                ariza.Durum = YeniDurum;

                if (!ariza.MudahaleBaslangicTarihi.HasValue && (YeniDurum == "Tamir Ediliyor" || YeniDurum == "Firmaya Gönderildi"))
                {
                    ariza.MudahaleBaslangicTarihi = DateTime.Now;
                }

                if (YeniDurum == "Firmaya Gönderildi" && !ariza.FirmaGonderimTarihi.HasValue)
                {
                    ariza.FirmaGonderimTarihi = DateTime.Now;
                }

                if (YeniDurum == "İşlem Bitti" && !ariza.CozumTarihi.HasValue)
                {
                    ariza.CozumTarihi = DateTime.Now;
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(TeknikerDashboard));
        }

        [HttpPost]
        [Authorize(Roles = "Yonetici")]
        public async Task<IActionResult> KullaniciOnayla(string onayUserId, string atanacakRol)
        {
            if (string.IsNullOrEmpty(onayUserId) || string.IsNullOrEmpty(atanacakRol))
            {
                TempData["HataMesaji"] = "Kullanıcı veya rol bilgisi eksik!";
                return RedirectToAction(nameof(YoneticiDashboard));
            }

            var user = await _userManager.FindByIdAsync(onayUserId);
            if (user == null)
            {
                return NotFound("Kullanıcı bulunamadı.");
            }

            var ekBilgi = await _context.KullaniciEkBilgileri.FirstOrDefaultAsync(k => k.UserId == onayUserId);
            if (ekBilgi != null)
            {
                ekBilgi.IsApproved = true;
                _context.KullaniciEkBilgileri.Update(ekBilgi);
                await _context.SaveChangesAsync();
            }

            await _userManager.AddToRoleAsync(user, atanacakRol);

            TempData["BasariMesaji"] = $"{user.Email} başarıyla onaylandı ve '{atanacakRol}' rolü atandı.";

            return RedirectToAction(nameof(YoneticiDashboard));
        }

        // POST: Arizalar/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Ariza ariza, IFormFile? fotografDosyasi)
        {
            if (ModelState.IsValid)
            {
                if (fotografDosyasi != null && fotografDosyasi.Length > 0)
                {
                    var uzanti = Path.GetExtension(fotografDosyasi.FileName).ToLower();

                    if (uzanti == ".jpg" || uzanti == ".jpeg" || uzanti == ".png")
                    {
                        var yeniDosyaAdi = Guid.NewGuid().ToString() + uzanti;
                        var klasorYolu = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/arizalar");

                        if (!Directory.Exists(klasorYolu))
                        {
                            Directory.CreateDirectory(klasorYolu);
                        }

                        var tamYol = Path.Combine(klasorYolu, yeniDosyaAdi);

                        using (var stream = new FileStream(tamYol, FileMode.Create))
                        {
                            await fotografDosyasi.CopyToAsync(stream);
                        }

                        ariza.FotografYolu = "/images/arizalar/" + yeniDosyaAdi;
                    }
                    else
                    {
                        ModelState.AddModelError("", "Güvenlik İhlali: Sisteme sadece .jpg ve .png formatında görsel yüklenebilir.");
                        ViewBag.Konumlar = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Konumlar, "Id", "Ad", ariza.KonumId);
                        return View(ariza);
                    }
                }

                ariza.BildirimTarihi = DateTime.Now;
                ariza.Durum = "Tekniker Alınmayı Bekliyor";

                // Arızayı kaydeden personelin e-postasını sisteme işliyoruz
                ariza.KaydedenKullaniciEmail = User.Identity?.Name;

                _context.Add(ariza);
                await _context.SaveChangesAsync();

                TempData["BasariMesaji"] = "Arıza bildiriminiz başarıyla oluşturuldu.";
                return RedirectToAction(nameof(Create));
            }

            ViewBag.Konumlar = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Konumlar, "Id", "Ad", ariza.KonumId);
            return View(ariza);
        }
    }
}
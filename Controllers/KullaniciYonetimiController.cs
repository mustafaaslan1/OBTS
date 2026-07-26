using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OBTS_Web.Data;
using OBTS_Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OBTS_Web.Controllers
{
    [Authorize(Roles = "Yonetici")]
    public class KullaniciYonetimiController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public KullaniciYonetimiController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var model = new List<KullaniciListeViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var ekBilgi = await _context.KullaniciEkBilgileri.FirstOrDefaultAsync(k => k.UserId == user.Id);

                model.Add(new KullaniciListeViewModel
                {
                    UserId = user.Id,
                    Email = user.Email,
                    AdSoyad = ekBilgi?.AdSoyad ?? "Belirtilmemiş",
                    DahiliHat = ekBilgi?.DahiliHat ?? "-",
                    IsApproved = ekBilgi?.IsApproved ?? false,
                    Rol = roles.FirstOrDefault() ?? "Rol Atanmamış"
                });
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Detay(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound("Kullanıcı kimliği belirtilmedi.");

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound("Kullanıcı bulunamadı.");

            var roles = await _userManager.GetRolesAsync(user);
            var ekBilgi = await _context.KullaniciEkBilgileri.FirstOrDefaultAsync(k => k.UserId == id);

            var model = new KullaniciDetayViewModel
            {
                UserId = user.Id,
                Email = user.Email,
                AdSoyad = ekBilgi?.AdSoyad,
                DahiliHat = ekBilgi?.DahiliHat,
                IsApproved = ekBilgi?.IsApproved ?? false,
                Rol = roles.FirstOrDefault()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProfilGuncelle(KullaniciDetayViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return NotFound("Kullanıcı bulunamadı.");

            var ekBilgi = await _context.KullaniciEkBilgileri.FirstOrDefaultAsync(k => k.UserId == model.UserId);
            bool isNew = false;

            if (ekBilgi == null)
            {
                ekBilgi = new KullaniciEkBilgi { UserId = model.UserId };
                isNew = true;
            }

            ekBilgi.AdSoyad = model.AdSoyad;
            ekBilgi.DahiliHat = model.DahiliHat;
            ekBilgi.IsApproved = model.IsApproved;

            if (isNew)
            {
                _context.KullaniciEkBilgileri.Add(ekBilgi);
            }
            else
            {
                _context.KullaniciEkBilgileri.Update(ekBilgi);
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.FirstOrDefault() != model.Rol && !string.IsNullOrEmpty(model.Rol))
            {
                if (currentRoles.Any())
                {
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);
                }
                await _userManager.AddToRoleAsync(user, model.Rol);
            }

            await _context.SaveChangesAsync();

            TempData["BasariMesaji"] = "Personel bilgileri başarıyla güncellendi.";
            return RedirectToAction(nameof(Detay), new { id = model.UserId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GeciciSifreUret(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("Kullanıcı bulunamadı.");

            Random rnd = new Random();
            string yeniSifre = $"Dhmi@{rnd.Next(100000, 999999)}";

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, yeniSifre);

            if (result.Succeeded)
            {
                var ekBilgi = await _context.KullaniciEkBilgileri.FirstOrDefaultAsync(k => k.UserId == userId);
                if (ekBilgi != null)
                {
                    ekBilgi.IsTemporaryPassword = true;
                    _context.Update(ekBilgi);
                    await _context.SaveChangesAsync();
                }

                TempData["OTP_Sifre"] = yeniSifre;
                TempData["BasariMesaji"] = "Geçici şifre başarıyla oluşturuldu.";
            }
            else
            {
                var hatalar = string.Join(" | ", result.Errors.Select(e => e.Description));
                TempData["HataMesaji"] = $"Şifre oluşturulamadı: {hatalar}";
            }

            return RedirectToAction(nameof(Detay), new { id = userId });
        }
    }
}
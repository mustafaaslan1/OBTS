// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using OBTS_Web.Data;
using Microsoft.EntityFrameworkCore;
using OBTS_Web.Models;

namespace OBTS_Web.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context; // Veritabanı bağlantısı eklendi

        public LoginModel(SignInManager<IdentityUser> signInManager, ILogger<LoginModel> logger, UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _signInManager = signInManager;
            _logger = logger;
            _userManager = userManager;
            _context = context; 
        }

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public IList<AuthenticationScheme>? ExternalLogins { get; set; }

        public string? ReturnUrl { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = default!;

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; } = default!;

            [Display(Name = "Beni Hatırla?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string? returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(Input.Email);

                    var ekBilgi = await _context.KullaniciEkBilgileri.FirstOrDefaultAsync(k => k.UserId == user.Id);

                    if (ekBilgi != null && !ekBilgi.IsApproved)
                    {
                        await _signInManager.SignOutAsync();
                        _logger.LogWarning("Onaylanmamış hesap giriş denemesi: {Email}", Input.Email);

                        ModelState.AddModelError(string.Empty, "Hesabınız henüz yönetici tarafından onaylanmamıştır. Lütfen sistem yöneticisinin onayını bekleyiniz.");
                        return Page();
                    }

                    if (ekBilgi != null && ekBilgi.IsTemporaryPassword)
                    {
                        _logger.LogInformation("Kullanıcı geçici şifre ile giriş yaptı, şifre değiştirme ekranına yönlendiriliyor.");

                        TempData["StatusMessage"] = "Bilgi Teknolojileri Teknikeri tarafından verilen tek kullanımlık şifreyi kullandınız. Sisteme devam edebilmek için lütfen yeni kalıcı şifrenizi belirleyiniz.";

                        return RedirectToPage("/Account/Manage/ChangePassword", new { area = "Identity" });
                    }

                    _logger.LogInformation("Kullanıcı giriş yaptı.");

                    // NORMAL GİRİŞ - ROL BAZLI YÖNLENDİRME
                    var roles = await _userManager.GetRolesAsync(user);

                    if (roles.Contains("Yonetici"))
                    {
                        return RedirectToAction("YoneticiDashboard", "Arizalar");
                    }
                    else if (roles.Contains("Tekniker"))
                    {
                        return RedirectToAction("TeknikerDashboard", "Arizalar");
                    }
                    else
                    {
                        return RedirectToAction("PersonelDashboard", "Arizalar");
                    }
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("Kullanıcı hesabı kilitlendi.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Geçersiz giriş denemesi. E-Posta veya şifrenizi kontrol ediniz.");
                    return Page();
                }
            }

            return Page();
        }
    }
}
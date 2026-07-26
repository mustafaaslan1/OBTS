using Microsoft.AspNetCore.Identity;

namespace OBTS_Web.Data
{
    public static class DbInitializer
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            string[] roleNames = { "Yonetici", "Tekniker", "Personel" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            var adminUser = new IdentityUser { UserName = "admin@dhmi.gov.tr", Email = "admin@dhmi.gov.tr" };
            if (userManager.Users.All(u => u.UserName != adminUser.UserName))
            {
                var result = await userManager.CreateAsync(adminUser, "DhmiAdmin123*");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Yonetici");
                }
            }

            var teknikerUser = new IdentityUser { UserName = "tekniker@dhmi.gov.tr", Email = "tekniker@dhmi.gov.tr" };
            if (userManager.Users.All(u => u.UserName != teknikerUser.UserName))
            {
                var result = await userManager.CreateAsync(teknikerUser, "DhmiTekniker123*");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(teknikerUser, "Tekniker");
                }
            }

            var personelUser = new IdentityUser { UserName = "personel@dhmi.gov.tr", Email = "personel@dhmi.gov.tr" };
            if (userManager.Users.All(u => u.UserName != personelUser.UserName))
            {
                var result = await userManager.CreateAsync(personelUser, "DhmiPersonel123*");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(personelUser, "Personel");
                }
            }
        }
    }
}
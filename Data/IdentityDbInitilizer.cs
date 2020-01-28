using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.Data
{
    public class IdentityDbInitilizer
    {
        public static async Task Initilize(IServiceProvider serviceProvider, string adminUserPW)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            string[] roleNames = { "Admin", "Student", "Instructor" };
            IdentityResult roleResult;
            foreach(var roleName in roleNames)
            {
                var roleExist = await RoleManager.RoleExistsAsync(roleName);
                if(!roleExist)
                {
                    roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
            var UserManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            IdentityUser adminUser = await UserManager.FindByEmailAsync("admin@contoso.com");
            if(adminUser == null)
            {
                adminUser = new IdentityUser()
                {
                    UserName = "admin@contoso.com",
                    Email = "admin@contoso.com"
                };
                await UserManager.CreateAsync(adminUser, adminUserPW);
                await UserManager.AddToRoleAsync(adminUser, "Admin");
                var code = await UserManager.GenerateEmailConfirmationTokenAsync(adminUser);
                var result = await UserManager.ConfirmEmailAsync(adminUser, code);
                await UserManager.SetLockoutEnabledAsync(adminUser, false);
            }
        }
    }
}

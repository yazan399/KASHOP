using KASHOP.BLL.Services;
using KASHOP.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace KASHOP.PL.Utils
{
    public class UserSeedData : ISeedData
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserSeedData(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task DataSeed()
        {
            await CreateUserAsync("superadmin1", "superadmin1@example.com", "Password123!", "SuperAdmin", "Super Admin");
            await CreateUserAsync("admin1", "admin1@example.com", "Password123!", "Admin", "Admin User");
            await CreateUserAsync("user1", "user1@example.com", "Password123!", "User", "User");

                }

        private async Task CreateUserAsync(string username, string email, string password,string role,string fullName)
        {

            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }
            if(await _userManager.FindByEmailAsync(email) != null)
            {
                return;
            }

            var user = new ApplicationUser
            {
                UserName = username,
                Email = email,
                FullName = fullName,
                EmailConfirmed = true,
                PhoneNumber= "1234567890",
                City = "vice City",
            };
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, role);
            }
        }
    }
}

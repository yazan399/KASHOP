using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KASHOP.DAL.Models;
using KASHOP.DAL.Data;

namespace KASHOP.PL.Utils
{
    public class RoleSeedData : ISeedData
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public RoleSeedData(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task DataSeed()
        {
            string[] roles = new string[] { "Super Admin", "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Removes the accidental legacy role while preserving access for its users.
            const string legacyRole = "SuperAdmin";
            if (await _roleManager.RoleExistsAsync(legacyRole))
            {
                var legacyUsers = await _userManager.GetUsersInRoleAsync(legacyRole);
                foreach (var user in legacyUsers)
                {
                    await _userManager.RemoveFromRoleAsync(user, legacyRole);
                    await _userManager.AddToRoleAsync(user, "Super Admin");
                }

                var legacyRoleEntity = await _roleManager.FindByNameAsync(legacyRole);
                if (legacyRoleEntity != null)
                {
                    await _roleManager.DeleteAsync(legacyRoleEntity);
                }
            }
        }
    }
}

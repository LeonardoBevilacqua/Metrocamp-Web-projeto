using Microsoft.AspNetCore.Identity;

namespace ChooseYourGame
{
    public static class IdentityDataInitializer
    {
        public static void SeedData(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
        }

        public static void SeedUsers(UserManager<IdentityUser> userManager)
        {
            if (userManager.FindByNameAsync("Admin").Result == null)
            {
                IdentityUser user = new IdentityUser();
                user.UserName = "Admin";
                user.Email = "leonardo_bevilacqua@hotmail.com";
                
                IdentityResult result = userManager.CreateAsync(user, "admin").Result;

                if(result.Succeeded){
                    userManager.AddToRoleAsync(user, "Admin").Wait();
                }
            }
        }

        public static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync("Admin").Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = "Admin";
                IdentityResult roleResult = roleManager.CreateAsync(role).Result;
            }

            if (!roleManager.RoleExistsAsync("CommonUser").Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = "CommonUser";
                IdentityResult roleResult = roleManager.CreateAsync(role).Result;
            }
        }
    }
}
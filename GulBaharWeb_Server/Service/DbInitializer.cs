using GulBahar_Common_Func_Lib;
using GulBahar_DataAcess_Lib.Data;
using GulBaharWeb_Server.Service.IService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GulBaharWeb_Server.Service
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;
        public DbInitializer(UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;

        }

        public void Initialize()
        {
            try
            {
                if(_db.Database.GetPendingMigrations().Count()>0)
                {
                    _db.Database.Migrate();
                }
                // if the role of admin exists then do nothing if it doesnt exist i will create role of admin and customer 
                if (! _roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
                {
                    _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
                }
                else
                {
                    // if the migration has already been executed so we just return back and not proceed further 
                    return;
                }
                // after the roles has been created I create the intial user which will be an admin user, only when i seed the database the first user
                // that gets created will be the admin user
                IdentityUser user = new()
                {
                    UserName = "zia@gulbahar.com",
                    Email = "zia@gulbahar.com",
                    EmailConfirmed = true,
                };
                // here i create the user and populate the details in the user object
                _userManager.CreateAsync(user, "Zia123*").GetAwaiter().GetResult();

                // once the user is create here i will assign the role 
                _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();


            }
            catch (Exception ex)
            {

            }

        }
    }
}

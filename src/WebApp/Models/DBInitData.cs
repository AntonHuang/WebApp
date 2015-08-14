using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.Framework.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.DomainModels.Customer;

namespace WebApp.Models
{
    public class DBInitData
    {
        private ApplicationDbContext ctx;
        private RoleManager<IdentityRole> roleManager;
        private UserManager<ApplicationUser> userManager;

        private ILogger Logger ;

        public DBInitData(ApplicationDbContext ctx,
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            ILoggerFactory loggerFactory)
        {
            this.ctx = ctx;
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.Logger = loggerFactory.CreateLogger<DBInitData>();
        }

        public void InitializeData()
        {
            if (ctx.Database.EnsureCreated())
            {
                try
                {
                    CreateRoles().Wait();
                    CreateUsers().Wait();

                }
                catch (Exception e)
                {
                    this.Logger.LogError(e.Message, e);
                    if (e.InnerException != null) {
                        this.Logger.LogError(e.InnerException.Message, e.InnerException);
                    }
                }
               
            }
        }

        private async Task CreateRoles()
        {
            if (this.roleManager.Roles.Count() == 0)
            {
                await this.roleManager.CreateAsync(new IdentityRole("Administrator"));
                await this.roleManager.CreateAsync(new IdentityRole("ShopManager"));
                ctx.SaveChanges();
            }
        }

        private async Task CreateUsers()
        {
            if (userManager.Users.Count() == 0)
            {
                await CreateUsers("BE0101001", "超级管理员" , "Administrator");
                await CreateUsers("BE0201001", "店长1", "ShopManager");
                await CreateUsers("BE0201002", "店长2", "ShopManager");
                await CreateUsers("BE0201003", "店长3", "ShopManager");
                await CreateUsers("BE0201004", "店长4", "ShopManager");
                await CreateUsers("BE0201005", "店长5", "ShopManager");
                await CreateUsers("BE0201006", "店长6", "ShopManager");
                await CreateUsers("BE0201007", "店长7", "ShopManager");
                await CreateUsers("BE0201008", "店长8", "ShopManager");
                await CreateUsers("BE0201009", "店长9", "ShopManager");
                await CreateUsers("BE0201010", "店长10", "ShopManager");
                await CreateUsers("BE0201011", "店长11", "ShopManager");
                await CreateUsers("BE0201012", "店长12", "ShopManager");
                await CreateUsers("BE0201013", "店长13", "ShopManager");
                await CreateUsers("BE0201014", "店长14", "ShopManager");
                await CreateUsers("BE0201015", "店长15", "ShopManager");
                await CreateUsers("BE0201016", "店长16", "ShopManager");
                await CreateUsers("BE0201017", "店长17", "ShopManager");
                await CreateUsers("BE0201018", "店长18", "ShopManager");
                await CreateUsers("BE0201019", "店长19", "ShopManager");
                await CreateUsers("BE0201020", "店长20", "ShopManager");

                ctx.SaveChanges();
            }

        }

        private async Task CreateUsers(string id, string name, string role)
        {
            ApplicationUser user = new ApplicationUser
            {
                Id = id,
                UserName = id
            };
            user.MemberInfo = new Member { MemberID = id, Name = name };
            ctx.Members.Add(user.MemberInfo);
            var rulst =  await userManager.CreateAsync(user, ApplicationUser.IINT_PASSWORD);
            await userManager.AddToRoleAsync(user, role);
            //await userManager.AddClaimAsync(user, new Claim("ManageStore", "Allowed"));
        }


    }
}

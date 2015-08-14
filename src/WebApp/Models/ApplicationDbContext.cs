using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.DomainModels.Customer;
using WebApp.DomainModels.Product;

namespace WebApp.Models
{
 
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        public virtual DbSet<Member> Members { get; set; }
        public virtual DbSet<Mattress> Mattress { get; set; }
        public virtual DbSet<ProductDesc> ProductDesc { get; set; }
        public virtual DbSet<SaleToCustomer> SaleToCustomer { get; set; }
        //public virtual DbSet<MemberPoint> MemberPoint { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            //one to one relationship between ApplicationUser and Member
            modelBuilder.Entity<Member>().Key(e => e.MemberID);
            modelBuilder.Entity<Member>().Property(e => e.MemberID)
                        .StoreGeneratedPattern(StoreGeneratedPattern.None);

            modelBuilder.Entity<ApplicationUser>()
                .Reference<Member>(appUser => appUser.MemberInfo).InverseReference();

            //one to many relationship
            modelBuilder.Entity<Member>(buildAction => {
                buildAction.Reference<Member>(m => m.Reference)
                                .InverseCollection(m => m.Candidates);
            });
            

            modelBuilder.Entity<ApplicationUser>()
                .Property(appUser => appUser.ChangedPassword).DefaultValue<bool>(0);


            //one to many relationship
            modelBuilder.Entity<Mattress>(buildAction => {
                buildAction.Reference<ProductDesc>(m => m.TypeDesc).InverseCollection();
            });

            //one to many relationship
            modelBuilder.Entity<SaleToCustomerDetail>(buildAction => {
                buildAction.Reference<SaleToCustomer>(d => d.Sale).InverseCollection(s => s.DetailItems);
            });

        }
    }
}

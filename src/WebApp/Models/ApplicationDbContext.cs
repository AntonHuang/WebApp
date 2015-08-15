using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.DomainModels.Core;
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
        public virtual DbSet<SaleToCustomerDetail> SaleToCustomeDetails { get; set; }
        public virtual DbSet<MemberPoint> MemberPoint { get; set; }
        public virtual DbSet<Setting> Settings { get; set; }
        public virtual DbSet<SettingHistory> SettingHistorys { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            
            modelBuilder.Entity<Member>(buildAction => {
                buildAction.Key(e => e.MemberID);
                buildAction.Property(e => e.MemberID).StoreGeneratedPattern(StoreGeneratedPattern.None);

                 //one to one relationship between ApplicationUser and Member
                buildAction.Reference<ApplicationUser>().InverseReference(appUser => appUser.MemberInfo);

                //one to many relationship
                buildAction.Reference<Member>(m => m.Reference)
                                .InverseCollection(m => m.Candidates);
            });
                  

            modelBuilder.Entity<ApplicationUser>()
                .Property(appUser => appUser.ChangedPassword).DefaultValue<bool>(0);


            modelBuilder.Entity<ProductDesc>(buildAction => {
                buildAction.Key(pd => pd.ID);
            });

            //one to many relationship
            modelBuilder.Entity<Mattress>(buildAction => {
                buildAction.Key(m => m.ID);
                buildAction.Reference<ProductDesc>(m => m.TypeDesc).InverseCollection();
            });

            //one to many relationship
            modelBuilder.Entity<SaleToCustomerDetail>(buildAction => {
                buildAction.Key(d => d.ID);
                buildAction.Property(d => d.ID).ForSqlServer().UseSequence();
                buildAction.Reference<SaleToCustomer>(d => d.Sale).InverseCollection(s => s.DetailItems);
            });

           
            modelBuilder.Entity<MemberPoint>(buildAction => {
                buildAction.Key(d => d.ID);

                //one to many relationship
                buildAction.Reference<Member>(mp => mp.Owner).InverseCollection(m => m.MemberPointItems);

                //one to many relationship
                buildAction.Reference<Member>(mp => mp.OperationBy).InverseCollection();

                //one to many relationship
                buildAction.Reference<Mattress>(mp => mp.Product).InverseCollection();

                //one to many relationship
                buildAction.Reference<Member>(mp => mp.ProductBuyer).InverseCollection();

            });

            modelBuilder.Entity<Setting>(buildAction => {
                buildAction.Key(s => s.ID);
            });

            modelBuilder.Entity<SettingHistory>(buildAction => {
                buildAction.Key(d => d.ID);
                buildAction.Property(d => d.ID).ForSqlServer().UseSequence();
            });
        }
    }
}

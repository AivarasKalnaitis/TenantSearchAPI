using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenantSearchAPI.Data.DTOs.Auth;
using TenantSearchAPI.Data.Entities;

namespace TenantSearchAPI
{
    public class TenantSearchContext : IdentityDbContext<User>
    {
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<Hobby> Hobbies { get; set; }
        public DbSet<Apartment> Apartments { get; set; }
        public DbSet<Landlord> Landlords { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Application> TenantApplications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            //modelBuilder.Entity<Review>()
            //   .HasOne<Tenant>()
            //   .WithMany(x => x.Reviews)
            //   .HasForeignKey(r => r.LandlordId);         
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB; Initial Catalog=TenantSearch");
        }
    }
}

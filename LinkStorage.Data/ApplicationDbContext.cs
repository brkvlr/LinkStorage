using LinkStorage.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace LinkStorage.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {

        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {

        }
        public virtual DbSet<AppUser> Users { get; set; }
        public virtual DbSet<AppUserType> UserTypes { get; set; }
        public virtual DbSet<Link> Links { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppUserType>().HasData(
                new AppUserType { Id = 1, Name = "Admin" },
                new AppUserType { Id = 2, Name = "User" }
                );
        }
    }
}
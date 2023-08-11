using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MVCapp.Models;

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext (DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NavBar>()
            .HasMany(e=>e.SubNavs)
            .WithOne(e=>e.NavBar)
            .HasForeignKey(e=>e.ParentId);
            
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<MVCapp.Models.Person> Person { get; set; } = default!;

        public DbSet<MVCapp.Models.Student> Student { get; set; } = default!;
    public DbSet<MVCapp.Models.NavBar> NavBar { get; set; } = default!;
    public DbSet<MVCapp.Models.SubNav> SubNav { get; set; } = default!;
}

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VicemMvcIdentity.Models;
using VicemMvcIdentity.Models.Entities;

namespace VicemMvcIdentity.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
     public DbSet<Employee> Employee { get; set; } = default!;
    public DbSet<MemberUnit> MemberUnit { get; set; } = default!;
}

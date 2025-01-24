using DotnetAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace DotnetAPI.Data
{
  public class CompanyDbContext : IdentityDbContext<ApplicationUser>
{
    public CompanyDbContext(DbContextOptions<CompanyDbContext> options)
        : base(options)
    {
    }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<CompanyEmployee> CompanyEmployee { get; set; }
    public DbSet<EmployeeProject> EmployeeProject { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Database=CompanyDB;Username=postgres;Password=rootroot");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // Company-Employee Many-to-Many
    modelBuilder.Entity<CompanyEmployee>()
        .HasKey(ce => new { ce.CompanyId, ce.EmployeeId });

    modelBuilder.Entity<CompanyEmployee>()
        .HasOne(ce => ce.Company)
        .WithMany(c => c.CompanyEmployee)
        .HasForeignKey(ce => ce.CompanyId);

    modelBuilder.Entity<CompanyEmployee>()
        .HasOne(ce => ce.Employee)
        .WithMany(e => e.CompanyEmployee)
        .HasForeignKey(ce => ce.EmployeeId);

    // Employee-Project Many-to-Many
    modelBuilder.Entity<EmployeeProject>()
        .HasKey(ep => new { ep.EmployeeId, ep.ProjectId });

    modelBuilder.Entity<EmployeeProject>()
        .HasOne(ep => ep.Employee)
        .WithMany(e => e.EmployeeProject)
        .HasForeignKey(ep => ep.EmployeeId);

    modelBuilder.Entity<EmployeeProject>()
        .HasOne(ep => ep.Project)
        .WithMany(p => p.EmployeeProject)
        .HasForeignKey(ep => ep.ProjectId);
}

}
}


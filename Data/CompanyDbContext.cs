using DotnetAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DotnetAPI.Data
{
  public class CompanyDbContext : DbContext
{
    public CompanyDbContext(DbContextOptions<CompanyDbContext> options)
        : base(options)
    {
    }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<CompanyEmployee> CompanyEmployees { get; set; }
    public DbSet<EmployeeProject> EmployeeProjects { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Database=CompanyDB;Username=postgres;Password=rootroot");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Company-Employee Many-to-Many
    modelBuilder.Entity<CompanyEmployee>()
        .HasKey(ce => new { ce.CompanyId, ce.EmployeeId });

    modelBuilder.Entity<CompanyEmployee>()
        .HasOne(ce => ce.Company)
        .WithMany(c => c.CompanyEmployees)
        .HasForeignKey(ce => ce.CompanyId);

    modelBuilder.Entity<CompanyEmployee>()
        .HasOne(ce => ce.Employee)
        .WithMany(e => e.CompanyEmployees)
        .HasForeignKey(ce => ce.EmployeeId);

    // Employee-Project Many-to-Many
    modelBuilder.Entity<EmployeeProject>()
        .HasKey(ep => new { ep.EmployeeId, ep.ProjectId });

    modelBuilder.Entity<EmployeeProject>()
        .HasOne(ep => ep.Employee)
        .WithMany(e => e.EmployeeProjects)
        .HasForeignKey(ep => ep.EmployeeId);

    modelBuilder.Entity<EmployeeProject>()
        .HasOne(ep => ep.Project)
        .WithMany(p => p.EmployeeProjects)
        .HasForeignKey(ep => ep.ProjectId);
}

}
}


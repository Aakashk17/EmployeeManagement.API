using EmployeeManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.API.Data;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    public DbSet<Employee> Employees => Set<Employee>();

    public DbSet<Department> Departments => Set<Department>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(user => user.Id);
            entity.HasIndex(user => user.Email).IsUnique();
            entity.Property(user => user.UserName).HasColumnName("Username").HasMaxLength(100).IsRequired();
            entity.Property(user => user.Email).HasMaxLength(150).IsRequired();
            entity.Property(user => user.PasswordHash).HasMaxLength(500).IsRequired();
            entity.Property(user => user.Role).HasMaxLength(20).IsRequired();
            entity.Property(user => user.CreatedDate).HasColumnType("datetime").HasDefaultValueSql("GETDATE()").IsRequired();
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable("Departments");
            entity.HasKey(department => department.Id);
            entity.Property(department => department.Name).HasMaxLength(100).IsRequired();
            entity.Property(department => department.Description).HasMaxLength(250);
            entity.Property(department => department.CreatedDate).HasColumnType("datetime").HasDefaultValueSql("GETDATE()").IsRequired();
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.ToTable("Employees");
            entity.HasKey(employee => employee.Id);
            entity.HasIndex(employee => employee.Email).IsUnique();
            entity.Property(employee => employee.Name).HasMaxLength(100).IsRequired();
            entity.Property(employee => employee.Email).HasMaxLength(150).IsRequired();
            entity.Property(employee => employee.Phone).HasMaxLength(20);
            entity.Property(employee => employee.Salary).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(employee => employee.IsActive).HasDefaultValue(true).IsRequired();
            entity.Property(employee => employee.CreatedDate).HasColumnType("datetime").HasDefaultValueSql("GETDATE()").IsRequired();

            entity.HasOne(employee => employee.Department)
                .WithMany(department => department.Employees)
                .HasForeignKey(employee => employee.DepartmentId)
                .HasConstraintName("FK_Employees_Departments")
                .OnDelete(DeleteBehavior.NoAction);
        });
    }
}

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EMS.Models;

public partial class EmsDbContext : DbContext
{
    public EmsDbContext()
    {
    }

    public EmsDbContext(DbContextOptions<EmsDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<DailyPresenty> DailyPresenties { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Feature> Features { get; set; }

    public virtual DbSet<LeaveRequest> LeaveRequests { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RoleFeatureAccess> RoleFeatureAccesses { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-NGALKNG\\SQLEXPRESS;Database=EMS_DB;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DailyPresenty>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__dailyPre__3213E83FCC55761F");

            entity.ToTable("dailyPresenty");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Empid).HasColumnName("empid");
            entity.Property(e => e.PunchIn)
                .HasColumnType("datetime")
                .HasColumnName("Punch_in");
            entity.Property(e => e.PunchOut)
                .HasColumnType("datetime")
                .HasColumnName("Punch_Out");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.DepartmentId).HasName("PK__Departme__B2079BEDF4EC1956");

            entity.Property(e => e.DepartmentName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__7AD04F116A2CBB8C");

            entity.HasIndex(e => e.AadhaarNumber, "UQ__Employee__72CF795906630BE2").IsUnique();

            entity.HasIndex(e => e.PanNumber, "UQ__Employee__7C38BFC86A6716A1").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Employee__A9D10534328455C7").IsUnique();

            entity.Property(e => e.AadhaarNumber)
                .HasMaxLength(12)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.PanNumber)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.PhotoPath)
                .HasMaxLength(300)
                .IsUnicode(false);

            entity.HasOne(d => d.Department).WithMany(p => p.Employees)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employees_Department");
        });

        modelBuilder.Entity<Feature>(entity =>
        {
            entity.HasKey(e => e.FeatureId).HasName("PK__Features__82230BC9059F1406");

            entity.HasIndex(e => e.FeatureKey, "UQ__Features__FCEFA330FAD4E845").IsUnique();

            entity.Property(e => e.AllowedRole)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FeatureKey)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FeatureLink)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.FeatureName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<LeaveRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LeaveReq__3213E83F3294D76C");

            entity.ToTable("LeaveRequest");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Empid).HasColumnName("empid");
            entity.Property(e => e.Fromdate).HasColumnType("datetime");
            entity.Property(e => e.Reason)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("reason");
            entity.Property(e => e.Status)
                .HasDefaultValue(false)
                .HasColumnName("status");
            entity.Property(e => e.Todate).HasColumnType("datetime");
            entity.Property(e => e.Typeofleave).HasColumnName("typeofleave");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.ProjectId).HasName("PK__Projects__761ABEF022F5C697");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ProjectName).HasMaxLength(200);
            entity.Property(e => e.Status).HasMaxLength(50);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1AA55E16D2");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B6160C074770F").IsUnique();

            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<RoleFeatureAccess>(entity =>
        {
            entity.HasKey(e => e.RoleFeatureId).HasName("PK__RoleFeat__0CA024413D4F8749");

            entity.ToTable("RoleFeatureAccess");

            entity.HasIndex(e => new { e.RoleId, e.FeatureId }, "UQ_Role_Feature").IsUnique();

            entity.HasOne(d => d.Feature).WithMany(p => p.RoleFeatureAccesses)
                .HasForeignKey(d => d.FeatureId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RoleFeature_Feature");

            entity.HasOne(d => d.Role).WithMany(p => p.RoleFeatureAccesses)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RoleFeature_Role");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4CDBA06207");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E4384C9B15").IsUnique();

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Employee).WithMany(p => p.Users)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK_Users_Employees");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Roles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

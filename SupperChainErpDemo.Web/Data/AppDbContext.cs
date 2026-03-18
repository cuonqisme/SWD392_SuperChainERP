using Microsoft.EntityFrameworkCore;
using SupperChainErpDemo.Web.Models;

namespace SupperChainErpDemo.Web.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Role> Roles => Set<Role>();

    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    public DbSet<UserAccount> Users => Set<UserAccount>();

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Product> Products => Set<Product>();

    public DbSet<Location> Locations => Set<Location>();

    public DbSet<TransferNote> TransferNotes => Set<TransferNote>();

    public DbSet<TransferNoteItem> TransferNoteItems => Set<TransferNoteItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(item => item.RoleId);
            entity.Property(item => item.RoleId).HasMaxLength(20);
            entity.Property(item => item.RoleName).HasMaxLength(50);
            entity.Property(item => item.Description).HasMaxLength(255);
            entity.Property(item => item.Status).HasConversion<string>().HasMaxLength(20);
            entity.Ignore(item => item.Permissions);
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(item => new { item.RoleId, item.PermissionCode });
            entity.Property(item => item.RoleId).HasMaxLength(20);
            entity.Property(item => item.PermissionCode).HasMaxLength(50);
            entity.HasOne(item => item.Role)
                .WithMany()
                .HasForeignKey(item => item.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserAccount>(entity =>
        {
            entity.HasKey(item => item.UserId);
            entity.Property(item => item.UserId).HasMaxLength(20);
            entity.Property(item => item.RoleId).HasMaxLength(20);
            entity.Property(item => item.Username).HasMaxLength(50);
            entity.Property(item => item.PasswordHash).HasMaxLength(255);
            entity.Property(item => item.FullName).HasMaxLength(100);
            entity.Property(item => item.Email).HasMaxLength(100);
            entity.Property(item => item.Phone).HasMaxLength(20);
            entity.Property(item => item.Status).HasConversion<string>().HasMaxLength(20);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(item => item.CategoryId);
            entity.Property(item => item.CategoryId).HasMaxLength(20);
            entity.Property(item => item.CategoryName).HasMaxLength(50);
            entity.Property(item => item.Description).HasMaxLength(500);
            entity.Property(item => item.SkuPrefix).HasMaxLength(20);
            entity.Property(item => item.Status).HasConversion<string>().HasMaxLength(20);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(item => item.ProductId);
            entity.Property(item => item.ProductId).HasMaxLength(20);
            entity.Property(item => item.CategoryId).HasMaxLength(20);
            entity.Property(item => item.ProductName).HasMaxLength(100);
            entity.Property(item => item.Sku).HasMaxLength(30);
            entity.Property(item => item.Barcode).HasMaxLength(30);
            entity.Property(item => item.BasePrice).HasPrecision(18, 2);
            entity.Property(item => item.Description).HasMaxLength(500);
            entity.Property(item => item.Status).HasConversion<string>().HasMaxLength(20);
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(item => item.LocationId);
            entity.Property(item => item.LocationId).HasMaxLength(20);
            entity.Property(item => item.LocationCode).HasMaxLength(20);
            entity.Property(item => item.LocationName).HasMaxLength(100);
            entity.Property(item => item.LocationType).HasMaxLength(30);
            entity.Property(item => item.Status).HasConversion<string>().HasMaxLength(20);
        });

        modelBuilder.Entity<TransferNote>(entity =>
        {
            entity.HasKey(item => item.TransferNoteId);
            entity.Property(item => item.TransferNoteId).HasMaxLength(20);
            entity.Property(item => item.TransferNo).HasMaxLength(20);
            entity.Property(item => item.SourceLocId).HasMaxLength(20);
            entity.Property(item => item.DestinationLocId).HasMaxLength(20);
            entity.Property(item => item.CreatedBy).HasMaxLength(20);
            entity.Property(item => item.ApprovedBy).HasMaxLength(20);
            entity.Property(item => item.Status).HasConversion<string>().HasMaxLength(30);
            entity.Property(item => item.Note).HasMaxLength(255);

            entity.HasOne<Location>()
                .WithMany()
                .HasForeignKey(item => item.SourceLocId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<Location>()
                .WithMany()
                .HasForeignKey(item => item.DestinationLocId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<UserAccount>()
                .WithMany()
                .HasForeignKey(item => item.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<UserAccount>()
                .WithMany()
                .HasForeignKey(item => item.ApprovedBy)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);
        });

        modelBuilder.Entity<TransferNoteItem>(entity =>
        {
            entity.HasKey(item => new { item.TransferNoteId, item.ProId });
            entity.Property(item => item.TransferNoteId).HasMaxLength(20);
            entity.Property(item => item.ProId).HasMaxLength(20);

            entity.HasOne<TransferNote>()
                .WithMany()
                .HasForeignKey(item => item.TransferNoteId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne<Product>()
                .WithMany()
                .HasForeignKey(item => item.ProId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}

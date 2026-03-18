using Microsoft.EntityFrameworkCore;
using SupperChainErpDemo.Web.Models;
using SupperChainErpDemo.Web.Services;

namespace SupperChainErpDemo.Web.Data;

public static class DbSeeder
{
    public static readonly IReadOnlyList<string> PermissionCatalog =
    [
        "User.Create",
        "User.View",
        "User.Update",
        "User.Status",
        "Role.Create",
        "Role.View",
        "Role.Update",
        "Role.Deactivate",
        "Category.Create",
        "Category.View",
        "Category.Update",
        "Category.Deactivate",
        "Product.Create",
        "Product.View",
        "Product.Update",
        "Product.Deactivate",
        "Transfer.Create",
        "Transfer.View",
        "Transfer.Update",
        "Transfer.Approve",
        "Transfer.ConfirmOut"
    ];

    public static async Task SeedAsync(AppDbContext dbContext)
    {
        await dbContext.Database.EnsureCreatedAsync();

        if (await dbContext.Roles.AnyAsync())
        {
            return;
        }

        var now = DateTime.UtcNow;

        var roles = new[]
        {
            new Role
            {
                RoleId = "ROL-001",
                RoleName = "System Administrator",
                Description = "Manages users, roles, master data, and governance settings.",
                Status = RecordStatus.Active,
                UpdatedDate = now.AddDays(-4)
            },
            new Role
            {
                RoleId = "ROL-002",
                RoleName = "Merchandise Manager",
                Description = "Owns product and category setup for supermarket branches.",
                Status = RecordStatus.Active,
                UpdatedDate = now.AddDays(-2)
            },
            new Role
            {
                RoleId = "ROL-003",
                RoleName = "Read-only Auditor",
                Description = "Reviews users, roles, products, and categories without editing rights.",
                Status = RecordStatus.Inactive,
                UpdatedDate = now.AddDays(-1)
            },
            new Role
            {
                RoleId = "ROL-004",
                RoleName = "Warehouse Staff",
                Description = "Creates transfer requests, updates drafts, and confirms transfer-out from source locations.",
                Status = RecordStatus.Active,
                UpdatedDate = now.AddDays(-2)
            },
            new Role
            {
                RoleId = "ROL-005",
                RoleName = "Chain Manager",
                Description = "Approves internal transfer notes to control stock movement between locations.",
                Status = RecordStatus.Active,
                UpdatedDate = now.AddDays(-1)
            },
            new Role
            {
                RoleId = "ROL-006",
                RoleName = "Store Manager",
                Description = "Monitors transfer note status and destination readiness for receiving teams.",
                Status = RecordStatus.Active,
                UpdatedDate = now.AddDays(-1)
            }
        };

        var rolePermissions = new[]
        {
            ("ROL-001", PermissionCatalog.ToArray()),
            ("ROL-002", new[] { "Category.View", "Category.Create", "Category.Update", "Product.View", "Product.Create", "Product.Update" }),
            ("ROL-003", new[] { "User.View", "Role.View", "Category.View", "Product.View", "Transfer.View" }),
            ("ROL-004", new[] { "Product.View", "Transfer.Create", "Transfer.View", "Transfer.Update", "Transfer.ConfirmOut" }),
            ("ROL-005", new[] { "Product.View", "Transfer.View", "Transfer.Approve" }),
            ("ROL-006", new[] { "Product.View", "Transfer.View" })
        }
        .SelectMany(item => item.Item2.Select(permission => new RolePermission
        {
            RoleId = item.Item1,
            PermissionCode = permission
        }));

        var categories = new[]
        {
            new Category
            {
                CategoryId = "CAT-001",
                CategoryName = "Fresh Produce",
                Description = "Vegetables, fruits, and daily replenishment products.",
                SkuPrefix = "FRESH",
                Status = RecordStatus.Active,
                CreatedDate = now.AddDays(-10),
                UpdatedDate = now.AddDays(-3)
            },
            new Category
            {
                CategoryId = "CAT-002",
                CategoryName = "Home Essentials",
                Description = "Cleaning, tissue, detergent, and household products.",
                SkuPrefix = "HOME",
                Status = RecordStatus.Active,
                CreatedDate = now.AddDays(-9),
                UpdatedDate = now.AddDays(-2)
            },
            new Category
            {
                CategoryId = "CAT-003",
                CategoryName = "Seasonal Campaign",
                Description = "Short-lived promotions and special event assortments.",
                SkuPrefix = "SEAS",
                Status = RecordStatus.Inactive,
                CreatedDate = now.AddDays(-7),
                UpdatedDate = now.AddDays(-1)
            }
        };

        var products = new[]
        {
            new Product
            {
                ProductId = "PRO-001",
                CategoryId = "CAT-001",
                ProductName = "Organic Roma Tomato",
                Sku = "FRESH-PRO-001",
                Barcode = "893001000001",
                BasePrice = 42000,
                Description = "Daily supply tomato batch for city-center stores.",
                Status = RecordStatus.Active,
                CreatedDate = now.AddDays(-6),
                UpdatedDate = now.AddDays(-2)
            },
            new Product
            {
                ProductId = "PRO-002",
                CategoryId = "CAT-002",
                ProductName = "Floor Cleaner 2L",
                Sku = "HOME-PRO-002",
                Barcode = "893001000002",
                BasePrice = 89000,
                Description = "Household cleaning liquid stocked across all branches.",
                Status = RecordStatus.Active,
                CreatedDate = now.AddDays(-5),
                UpdatedDate = now.AddDays(-2)
            },
            new Product
            {
                ProductId = "PRO-003",
                CategoryId = "CAT-003",
                ProductName = "Tet Gift Basket Lite",
                Sku = "SEAS-PRO-003",
                Barcode = "893001000003",
                BasePrice = 199000,
                Description = "Promotional bundle used for seasonal sales campaigns.",
                Status = RecordStatus.Inactive,
                CreatedDate = now.AddDays(-4),
                UpdatedDate = now.AddDays(-1)
            }
        };

        var locations = new[]
        {
            new Location
            {
                LocationId = "LOC-001",
                LocationCode = "WH-CENTRAL",
                LocationName = "Central Distribution Warehouse",
                LocationType = "Warehouse",
                Status = RecordStatus.Active
            },
            new Location
            {
                LocationId = "LOC-002",
                LocationCode = "STR-D1",
                LocationName = "District 1 Store",
                LocationType = "Store",
                Status = RecordStatus.Active
            },
            new Location
            {
                LocationId = "LOC-003",
                LocationCode = "STR-TD",
                LocationName = "Thu Duc Store",
                LocationType = "Store",
                Status = RecordStatus.Active
            },
            new Location
            {
                LocationId = "LOC-004",
                LocationCode = "WH-BACKUP",
                LocationName = "Backup Warehouse",
                LocationType = "Warehouse",
                Status = RecordStatus.Inactive
            }
        };

        var users = new[]
        {
            new UserAccount
            {
                UserId = "USR-001",
                RoleId = "ROL-001",
                Username = "admin.erp",
                PasswordHash = PasswordHasher.Hash("Demo@123"),
                FullName = "Linh Tran",
                Email = "linh.tran@supperchain.local",
                Phone = "0901000101",
                Status = RecordStatus.Active,
                CreatedDate = now.AddDays(-12),
                UpdatedDate = now.AddDays(-1)
            },
            new UserAccount
            {
                UserId = "USR-002",
                RoleId = "ROL-002",
                Username = "thu.merch",
                PasswordHash = PasswordHasher.Hash("Demo@123"),
                FullName = "Thu Nguyen",
                Email = "thu.nguyen@supperchain.local",
                Phone = "0901000102",
                Status = RecordStatus.Active,
                CreatedDate = now.AddDays(-11),
                UpdatedDate = now.AddDays(-2)
            },
            new UserAccount
            {
                UserId = "USR-003",
                RoleId = "ROL-003",
                Username = "khoa.audit",
                PasswordHash = PasswordHasher.Hash("Demo@123"),
                FullName = "Khoa Pham",
                Email = "khoa.pham@supperchain.local",
                Phone = "0901000103",
                Status = RecordStatus.Inactive,
                CreatedDate = now.AddDays(-8),
                UpdatedDate = now.AddDays(-3)
            },
            new UserAccount
            {
                UserId = "USR-004",
                RoleId = "ROL-004",
                Username = "huy.warehouse",
                PasswordHash = PasswordHasher.Hash("Demo@123"),
                FullName = "Huy Le",
                Email = "huy.le@supperchain.local",
                Phone = "0901000104",
                Status = RecordStatus.Active,
                CreatedDate = now.AddDays(-7),
                UpdatedDate = now.AddDays(-1)
            },
            new UserAccount
            {
                UserId = "USR-005",
                RoleId = "ROL-005",
                Username = "mai.chain",
                PasswordHash = PasswordHasher.Hash("Demo@123"),
                FullName = "Mai Vo",
                Email = "mai.vo@supperchain.local",
                Phone = "0901000105",
                Status = RecordStatus.Active,
                CreatedDate = now.AddDays(-6),
                UpdatedDate = now.AddDays(-1)
            },
            new UserAccount
            {
                UserId = "USR-006",
                RoleId = "ROL-006",
                Username = "nhi.store",
                PasswordHash = PasswordHasher.Hash("Demo@123"),
                FullName = "Nhi Dang",
                Email = "nhi.dang@supperchain.local",
                Phone = "0901000106",
                Status = RecordStatus.Active,
                CreatedDate = now.AddDays(-5),
                UpdatedDate = now.AddDays(-1)
            }
        };

        var transferNotes = new[]
        {
            new TransferNote
            {
                TransferNoteId = "TRN-001",
                TransferNo = "TN-001",
                SourceLocId = "LOC-001",
                DestinationLocId = "LOC-002",
                CreatedBy = "USR-004",
                CreatedDate = now.AddDays(-2),
                Status = TransferNoteStatus.Draft,
                Note = "Urgent replenishment for weekend produce demand."
            },
            new TransferNote
            {
                TransferNoteId = "TRN-002",
                TransferNo = "TN-002",
                SourceLocId = "LOC-001",
                DestinationLocId = "LOC-003",
                CreatedBy = "USR-004",
                ApprovedBy = "USR-005",
                CreatedDate = now.AddDays(-3),
                ApprovedDate = now.AddDays(-2),
                Status = TransferNoteStatus.Approved,
                Note = "Cleaning supplies transfer approved for Thu Duc branch."
            },
            new TransferNote
            {
                TransferNoteId = "TRN-003",
                TransferNo = "TN-003",
                SourceLocId = "LOC-002",
                DestinationLocId = "LOC-003",
                CreatedBy = "USR-004",
                ApprovedBy = "USR-005",
                CreatedDate = now.AddDays(-4),
                ApprovedDate = now.AddDays(-4),
                Status = TransferNoteStatus.Rejected,
                Note = "Rejected because the source store cannot release enough stock."
            }
        };

        var transferNoteItems = new[]
        {
            new TransferNoteItem
            {
                TransferNoteId = "TRN-001",
                ProId = "PRO-001",
                Quantity = 18
            },
            new TransferNoteItem
            {
                TransferNoteId = "TRN-001",
                ProId = "PRO-002",
                Quantity = 6
            },
            new TransferNoteItem
            {
                TransferNoteId = "TRN-002",
                ProId = "PRO-002",
                Quantity = 12
            },
            new TransferNoteItem
            {
                TransferNoteId = "TRN-003",
                ProId = "PRO-001",
                Quantity = 40
            }
        };

        await dbContext.Roles.AddRangeAsync(roles);
        await dbContext.RolePermissions.AddRangeAsync(rolePermissions);
        await dbContext.Categories.AddRangeAsync(categories);
        await dbContext.Products.AddRangeAsync(products);
        await dbContext.Locations.AddRangeAsync(locations);
        await dbContext.Users.AddRangeAsync(users);
        await dbContext.TransferNotes.AddRangeAsync(transferNotes);
        await dbContext.TransferNoteItems.AddRangeAsync(transferNoteItems);
        await dbContext.SaveChangesAsync();
    }
}

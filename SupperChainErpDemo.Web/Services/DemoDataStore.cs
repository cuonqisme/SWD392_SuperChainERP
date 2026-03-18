using SupperChainErpDemo.Web.Models;

namespace SupperChainErpDemo.Web.Services;

public class DemoDataStore
{
    private readonly object _syncRoot = new();
    private int _roleSequence = 3;
    private int _userSequence = 3;
    private int _categorySequence = 3;
    private int _productSequence = 3;

    public DemoDataStore()
    {
        Seed();
    }

    public List<Role> Roles { get; } = [];

    public List<UserAccount> Users { get; } = [];

    public List<Category> Categories { get; } = [];

    public List<Product> Products { get; } = [];

    public IReadOnlyList<string> PermissionCatalog { get; } =
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
        "Product.Deactivate"
    ];

    public string NextRoleId()
    {
        lock (_syncRoot)
        {
            _roleSequence++;
            return $"ROL-{_roleSequence:000}";
        }
    }

    public string NextUserId()
    {
        lock (_syncRoot)
        {
            _userSequence++;
            return $"USR-{_userSequence:000}";
        }
    }

    public string NextCategoryId()
    {
        lock (_syncRoot)
        {
            _categorySequence++;
            return $"CAT-{_categorySequence:000}";
        }
    }

    public string NextProductId()
    {
        lock (_syncRoot)
        {
            _productSequence++;
            return $"PRO-{_productSequence:000}";
        }
    }

    private void Seed()
    {
        var adminRole = new Role
        {
            RoleId = "ROL-001",
            RoleName = "System Administrator",
            Description = "Manages users, roles, master data, and governance settings.",
            Permissions = PermissionCatalog.ToList(),
            Status = RecordStatus.Active,
            UpdatedDate = DateTime.UtcNow.AddDays(-4)
        };

        var merchandiserRole = new Role
        {
            RoleId = "ROL-002",
            RoleName = "Merchandise Manager",
            Description = "Owns product and category setup for supermarket branches.",
            Permissions = ["Category.View", "Category.Create", "Category.Update", "Product.View", "Product.Create", "Product.Update"],
            Status = RecordStatus.Active,
            UpdatedDate = DateTime.UtcNow.AddDays(-2)
        };

        var auditorRole = new Role
        {
            RoleId = "ROL-003",
            RoleName = "Read-only Auditor",
            Description = "Reviews users, roles, products, and categories without editing rights.",
            Permissions = ["User.View", "Role.View", "Category.View", "Product.View"],
            Status = RecordStatus.Inactive,
            UpdatedDate = DateTime.UtcNow.AddDays(-1)
        };

        Roles.AddRange([adminRole, merchandiserRole, auditorRole]);

        Categories.AddRange(
        [
            new Category
            {
                CategoryId = "CAT-001",
                CategoryName = "Fresh Produce",
                Description = "Vegetables, fruits, and daily replenishment products.",
                SkuPrefix = "FRESH",
                Status = RecordStatus.Active,
                CreatedDate = DateTime.UtcNow.AddDays(-10),
                UpdatedDate = DateTime.UtcNow.AddDays(-3)
            },
            new Category
            {
                CategoryId = "CAT-002",
                CategoryName = "Home Essentials",
                Description = "Cleaning, tissue, detergent, and household products.",
                SkuPrefix = "HOME",
                Status = RecordStatus.Active,
                CreatedDate = DateTime.UtcNow.AddDays(-9),
                UpdatedDate = DateTime.UtcNow.AddDays(-2)
            },
            new Category
            {
                CategoryId = "CAT-003",
                CategoryName = "Seasonal Campaign",
                Description = "Short-lived promotions and special event assortments.",
                SkuPrefix = "SEAS",
                Status = RecordStatus.Inactive,
                CreatedDate = DateTime.UtcNow.AddDays(-7),
                UpdatedDate = DateTime.UtcNow.AddDays(-1)
            }
        ]);

        Products.AddRange(
        [
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
                CreatedDate = DateTime.UtcNow.AddDays(-6),
                UpdatedDate = DateTime.UtcNow.AddDays(-2)
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
                CreatedDate = DateTime.UtcNow.AddDays(-5),
                UpdatedDate = DateTime.UtcNow.AddDays(-2)
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
                CreatedDate = DateTime.UtcNow.AddDays(-4),
                UpdatedDate = DateTime.UtcNow.AddDays(-1)
            }
        ]);

        Users.AddRange(
        [
            new UserAccount
            {
                UserId = "USR-001",
                RoleId = adminRole.RoleId,
                Username = "admin.erp",
                PasswordHash = HashPassword("Demo@123"),
                FullName = "Linh Tran",
                Email = "linh.tran@supperchain.local",
                Phone = "0901000101",
                Status = RecordStatus.Active,
                CreatedDate = DateTime.UtcNow.AddDays(-12),
                UpdatedDate = DateTime.UtcNow.AddDays(-1)
            },
            new UserAccount
            {
                UserId = "USR-002",
                RoleId = merchandiserRole.RoleId,
                Username = "thu.merch",
                PasswordHash = HashPassword("Demo@123"),
                FullName = "Thu Nguyen",
                Email = "thu.nguyen@supperchain.local",
                Phone = "0901000102",
                Status = RecordStatus.Active,
                CreatedDate = DateTime.UtcNow.AddDays(-11),
                UpdatedDate = DateTime.UtcNow.AddDays(-2)
            },
            new UserAccount
            {
                UserId = "USR-003",
                RoleId = auditorRole.RoleId,
                Username = "khoa.audit",
                PasswordHash = HashPassword("Demo@123"),
                FullName = "Khoa Pham",
                Email = "khoa.pham@supperchain.local",
                Phone = "0901000103",
                Status = RecordStatus.Inactive,
                CreatedDate = DateTime.UtcNow.AddDays(-8),
                UpdatedDate = DateTime.UtcNow.AddDays(-3)
            }
        ]);
    }

    public static string HashPassword(string password) => $"HASH::{password}";
}

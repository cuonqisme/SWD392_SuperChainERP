# Use Case Mapping

Tai lieu nay map use case trong doc sang cac thanh phan code de de trace voi communication diagram.

## Format doc nhanh

Moi use case duoc map theo chuoi:

`Boundary UI -> Controller -> Coordinator -> Service -> Data / Supporting Service`

## User Management

### UC-01 Create User

- `Views/Users/Create.cshtml` -> `UsersController.Create()` -> `IUserManagementCoordinator.CreateUser()` -> `IUserService.CreateUser()` -> `AppDbContext.Users`, `AppDbContext.Roles`

### UC-02 View User

- `Views/Users/Index.cshtml`, `Views/Users/Details.cshtml` -> `UsersController.Index()/Details()` -> `ShowUserList()/ShowUserDetails()` -> `GetUserList()/GetUserDetails()` -> `AppDbContext.Users`, `AppDbContext.Roles`

### UC-03 Update User

- `Views/Users/Edit.cshtml` -> `UsersController.Edit()` -> `PrepareUpdateUser()/UpdateUser()` -> `PrepareUpdateUser()/UpdateUser()` -> `AppDbContext.Users`, `AppDbContext.Roles`

### UC-04 Update Status User

- `Views/Users/Index.cshtml` -> `UsersController.ChangeStatus()` -> `UpdateUserStatus()` -> `UpdateUserStatus()` -> `AppDbContext.Users`

## Role Management

### UC-05 Create Role

- `Views/Roles/Create.cshtml` -> `RolesController.Create()` -> `PrepareCreateRole()/CreateRole()` -> `PrepareCreateRole()/CreateRole()` -> `AppDbContext.Roles`, `AppDbContext.RolePermissions`, `PermissionService`

### UC-06 View Role

- `Views/Roles/Index.cshtml`, `Views/Roles/Details.cshtml` -> `RolesController.Index()/Details()` -> `ShowRoleList()/ShowRoleDetails()` -> `GetRoleList()/GetRoleDetails()` -> `AppDbContext.Roles`, `AppDbContext.RolePermissions`

### UC-07 Update Role

- `Views/Roles/Edit.cshtml` -> `RolesController.Edit()` -> `PrepareUpdateRole()/UpdateRole()` -> `PrepareUpdateRole()/UpdateRole()` -> `AppDbContext.Roles`, `AppDbContext.RolePermissions`, `PermissionService`

### UC-08 Deactivate Role

- `Views/Roles/Index.cshtml` -> `RolesController.Deactivate()` -> `DeactivateRole()` -> `DeactivateRole()` -> `AppDbContext.Roles`, `AppDbContext.Users`

## Product Management

### UC-09 Create Product

- `Views/Products/Create.cshtml` -> `ProductsController.Create()` -> `PrepareCreateProduct()/CreateProduct()` -> `PrepareCreateProduct()/CreateProduct()` -> `AppDbContext.Products`, `AppDbContext.Categories`

### UC-10 View Product

- `Views/Products/Index.cshtml`, `Views/Products/Details.cshtml` -> `ProductsController.Index()/Details()` -> `ShowProductList()/ShowProductDetails()` -> `GetProductList()/GetProductDetails()` -> `AppDbContext.Products`, `AppDbContext.Categories`

### UC-11 Update Product

- `Views/Products/Edit.cshtml` -> `ProductsController.Edit()` -> `PrepareUpdateProduct()/UpdateProduct()` -> `PrepareUpdateProduct()/UpdateProduct()` -> `AppDbContext.Products`, `AppDbContext.Categories`

### UC-12 Deactivate Product

- `Views/Products/Index.cshtml` -> `ProductsController.Deactivate()` -> `DeactivateProduct()` -> `DeactivateProduct()` -> `AppDbContext.Products`

## Category Management

### UC-13 Create Category

- `Views/Categories/Create.cshtml` -> `CategoriesController.Create()` -> `PrepareCreateCategory()/CreateCategory()` -> `PrepareCreateCategory()/CreateCategory()` -> `AppDbContext.Categories`

### UC-14 View Category

- `Views/Categories/Index.cshtml`, `Views/Categories/Details.cshtml` -> `CategoriesController.Index()/Details()` -> `ShowCategoryList()/ShowCategoryDetails()` -> `GetCategoryList()/GetCategoryDetails()` -> `AppDbContext.Categories`, `AppDbContext.Products`

### UC-15 Update Category

- `Views/Categories/Edit.cshtml` -> `CategoriesController.Edit()` -> `PrepareUpdateCategory()/UpdateCategory()` -> `PrepareUpdateCategory()/UpdateCategory()` -> `AppDbContext.Categories`, `AppDbContext.Products`

### UC-16 Deactivate Category

- `Views/Categories/Index.cshtml` -> `CategoriesController.Deactivate()` -> `DeactivateCategory()` -> `DeactivateCategory()` -> `AppDbContext.Categories`, `AppDbContext.Products`

## Transfer Note Management

### UC-41 Create Transfer Note

- `Views/TransferNotes/Create.cshtml` -> `TransferNotesController.Create()` -> `InitializeTransferDraft()/CreateTransferNote()` -> `TransferService.CreateTransferNote()` -> `LocationService`, `ProductService`, `InventoryQueryService`, `ValidationService`, `AppDbContext.TransferNotes`, `AppDbContext.TransferNoteItems`

### UC-42 View Transfer Note

- `Views/TransferNotes/Index.cshtml`, `Views/TransferNotes/Details.cshtml` -> `TransferNotesController.Index()/Details()` -> `ShowTransferNoteList()/ShowTransferNoteDetails()` -> `TransferService.GetTransferNotes()/GetTransferNote()` -> `FilterService`, `AppDbContext.TransferNotes`, `AppDbContext.TransferNoteItems`

### UC-43 Update Transfer Note

- `Views/TransferNotes/Edit.cshtml` -> `TransferNotesController.Edit()` -> `PrepareTransferUpdate()/UpdateTransferNote()` -> `TransferService.UpdateTransferNote()` -> `ValidationService`, `InventoryQueryService`, `AppDbContext.TransferNotes`, `AppDbContext.TransferNoteItems`

### UC-44 Approve Transfer Note

- `Views/TransferNotes/Index.cshtml`, `Views/TransferNotes/Details.cshtml` -> `TransferNotesController.Approve()/Reject()` -> `ApproveTransferNote()/RejectTransferNote()` -> `TransferService.ReviewTransferNote()` -> `InventoryService`, `AppDbContext.TransferNotes`

### UC-45 Confirm Transfer Out

- `Views/TransferNotes/Details.cshtml` -> `TransferNotesController.ConfirmTransferOut()` -> `ConfirmTransferOut()` -> `TransferService.ConfirmTransferOut()` -> `InventoryService`, `AuditLogService`, `AppDbContext.TransferNotes`

## Communication Diagram Alignment

Participant trong doc duoc map sang code nhu sau:

- `TransferInteraction` -> `Views/TransferNotes/*` + `TransferNotesController`
- `TransferCoordinator` -> `TransferCoordinator`
- `TransferService` -> `TransferService`
- `LocationService` -> `LocationService`
- `ProductService` -> `ProductService`
- `InventoryQueryService` -> `InventoryQueryService`
- `FilterService` -> `FilterService`
- `ValidationService` -> `ValidationService`
- `InventoryService` -> `InventoryService`
- `AuditLogService` -> `AuditLogService`

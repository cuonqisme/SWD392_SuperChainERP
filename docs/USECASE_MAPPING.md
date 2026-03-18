# Use Case Mapping

Tai lieu nay map use case trong doc sang cac thanh phan code de de trace voi communication diagram.

## User Management

### UC-01 Create User

- `Boundary UI`: `Views/Users/Create.cshtml`
- `Controller`: `UsersController.Create()`
- `Coordinator`: `IUserManagementCoordinator.PrepareCreateUser()`, `CreateUser()`
- `Service`: `IUserService.PrepareCreateUser()`, `CreateUser()`
- `Data`: `AppDbContext.Users`, `AppDbContext.Roles`

### UC-02 View User

- `Boundary UI`: `Views/Users/Index.cshtml`, `Views/Users/Details.cshtml`
- `Controller`: `UsersController.Index()`, `UsersController.Details()`
- `Coordinator`: `ShowUserList()`, `ShowUserDetails()`
- `Service`: `GetUserList()`, `GetUserDetails()`

### UC-03 Update User

- `Boundary UI`: `Views/Users/Edit.cshtml`
- `Controller`: `UsersController.Edit()`
- `Coordinator`: `PrepareUpdateUser()`, `UpdateUser()`
- `Service`: `PrepareUpdateUser()`, `UpdateUser()`

### UC-04 Update Status User

- `Boundary UI`: `Views/Users/Index.cshtml`
- `Controller`: `UsersController.ChangeStatus()`
- `Coordinator`: `UpdateUserStatus()`
- `Service`: `UpdateUserStatus()`

## Role Management

### UC-05 Create Role

- `Boundary UI`: `Views/Roles/Create.cshtml`
- `Controller`: `RolesController.Create()`
- `Coordinator`: `PrepareCreateRole()`, `CreateRole()`
- `Service`: `PrepareCreateRole()`, `CreateRole()`
- `Supporting Service`: `PermissionService`

### UC-06 View Role

- `Boundary UI`: `Views/Roles/Index.cshtml`, `Views/Roles/Details.cshtml`
- `Controller`: `RolesController.Index()`, `RolesController.Details()`
- `Coordinator`: `ShowRoleList()`, `ShowRoleDetails()`
- `Service`: `GetRoleList()`, `GetRoleDetails()`

### UC-07 Update Role

- `Boundary UI`: `Views/Roles/Edit.cshtml`
- `Controller`: `RolesController.Edit()`
- `Coordinator`: `PrepareUpdateRole()`, `UpdateRole()`
- `Service`: `PrepareUpdateRole()`, `UpdateRole()`

### UC-08 Deactivate Role

- `Boundary UI`: `Views/Roles/Index.cshtml`
- `Controller`: `RolesController.Deactivate()`
- `Coordinator`: `DeactivateRole()`
- `Service`: `DeactivateRole()`

## Product Management

### UC-09 Create Product

- `Boundary UI`: `Views/Products/Create.cshtml`
- `Controller`: `ProductsController.Create()`
- `Coordinator`: `PrepareCreateProduct()`, `CreateProduct()`
- `Service`: `PrepareCreateProduct()`, `CreateProduct()`

### UC-10 View Product

- `Boundary UI`: `Views/Products/Index.cshtml`, `Views/Products/Details.cshtml`
- `Controller`: `ProductsController.Index()`, `ProductsController.Details()`
- `Coordinator`: `ShowProductList()`, `ShowProductDetails()`
- `Service`: `GetProductList()`, `GetProductDetails()`

### UC-11 Update Product

- `Boundary UI`: `Views/Products/Edit.cshtml`
- `Controller`: `ProductsController.Edit()`
- `Coordinator`: `PrepareUpdateProduct()`, `UpdateProduct()`
- `Service`: `PrepareUpdateProduct()`, `UpdateProduct()`

### UC-12 Deactivate Product

- `Boundary UI`: `Views/Products/Index.cshtml`
- `Controller`: `ProductsController.Deactivate()`
- `Coordinator`: `DeactivateProduct()`
- `Service`: `DeactivateProduct()`

## Category Management

### UC-13 Create Category

- `Boundary UI`: `Views/Categories/Create.cshtml`
- `Controller`: `CategoriesController.Create()`
- `Coordinator`: `PrepareCreateCategory()`, `CreateCategory()`
- `Service`: `PrepareCreateCategory()`, `CreateCategory()`

### UC-14 View Category

- `Boundary UI`: `Views/Categories/Index.cshtml`, `Views/Categories/Details.cshtml`
- `Controller`: `CategoriesController.Index()`, `CategoriesController.Details()`
- `Coordinator`: `ShowCategoryList()`, `ShowCategoryDetails()`
- `Service`: `GetCategoryList()`, `GetCategoryDetails()`

### UC-15 Update Category

- `Boundary UI`: `Views/Categories/Edit.cshtml`
- `Controller`: `CategoriesController.Edit()`
- `Coordinator`: `PrepareUpdateCategory()`, `UpdateCategory()`
- `Service`: `PrepareUpdateCategory()`, `UpdateCategory()`

### UC-16 Deactivate Category

- `Boundary UI`: `Views/Categories/Index.cshtml`
- `Controller`: `CategoriesController.Deactivate()`
- `Coordinator`: `DeactivateCategory()`
- `Service`: `DeactivateCategory()`

## Transfer Note Management

### UC-41 Create Transfer Note

- `Boundary UI`: `Views/TransferNotes/Create.cshtml`
- `Controller`: `TransferNotesController.Create()`
- `Coordinator`: `InitializeTransferDraft()`, `CreateTransferNote()`
- `Service`: `TransferService.CreateTransferNote()`
- `Supporting Service`: `LocationService`, `ProductService`, `InventoryQueryService`, `ValidationService`

### UC-42 View Transfer Note

- `Boundary UI`: `Views/TransferNotes/Index.cshtml`, `Views/TransferNotes/Details.cshtml`
- `Controller`: `TransferNotesController.Index()`, `TransferNotesController.Details()`
- `Coordinator`: `ShowTransferNoteList()`, `ShowTransferNoteDetails()`
- `Service`: `TransferService.GetTransferNotes()`, `TransferService.GetTransferNote()`
- `Supporting Service`: `FilterService`

### UC-43 Update Transfer Note

- `Boundary UI`: `Views/TransferNotes/Edit.cshtml`
- `Controller`: `TransferNotesController.Edit()`
- `Coordinator`: `PrepareTransferUpdate()`, `UpdateTransferNote()`
- `Service`: `TransferService.UpdateTransferNote()`
- `Supporting Service`: `ValidationService`, `InventoryQueryService`

### UC-44 Approve Transfer Note

- `Boundary UI`: `Views/TransferNotes/Index.cshtml`, `Views/TransferNotes/Details.cshtml`
- `Controller`: `TransferNotesController.Approve()`, `TransferNotesController.Reject()`
- `Coordinator`: `ApproveTransferNote()`, `RejectTransferNote()`
- `Service`: `TransferService.ReviewTransferNote()`
- `Supporting Service`: `InventoryService`

### UC-45 Confirm Transfer Out

- `Boundary UI`: `Views/TransferNotes/Details.cshtml`
- `Controller`: `TransferNotesController.ConfirmTransferOut()`
- `Coordinator`: `ConfirmTransferOut()`
- `Service`: `TransferService.ConfirmTransferOut()`
- `Supporting Service`: `InventoryService`, `AuditLogService`

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

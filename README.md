# SWD392 SuperChain ERP Demo

Demo website `ASP.NET Core MVC` cho SupperChain ERP, bam theo use case, communication diagram, va database design trong tai lieu mon hoc.

## Pham vi da trien khai

- `UC-01` den `UC-16`: User, Role, Product, Category
- `UC-41` den `UC-45`: Transfer Note

## Kien truc tong quan

Luong xu ly chinh trong he thong:

`Boundary UI -> Controller -> Coordinator -> Service -> AppDbContext -> Notification`

Map participant trong code:

- `Boundary UI`: Razor Views trong `Views/*`
- `Boundary Controller`: MVC Controllers trong `Controllers/*`
- `Coordinator`: `*ManagementCoordinator`, `TransferCoordinator`
- `Business Service`: `UserService`, `RoleService`, `CategoryService`, `ProductService`, `TransferService`
- `Supporting Service`: `PermissionService`, `FilterService`, `ValidationService`, `InventoryQueryService`, `InventoryService`, `AuditLogService`
- `Persistence`: `AppDbContext` + EF Core
- `Notification`: `NotificationService`

## Patterns da ap dung

### 1. MVC

- Ap dung qua `Models`, `Views`, `Controllers`
- Vi du: `Controllers/UsersController.cs`, `Views/Users/*`, `Data/AppDbContext.cs`
- Ly do dung: tach rieng giao dien, dieu huong request, va du lieu nghiep vu; phu hop voi he thong ERP nhieu man CRUD

### 2. Service Layer

- Ap dung qua cac lop `UserService`, `RoleService`, `CategoryService`, `ProductService`, `TransferService`
- Ly do dung: business rule khong nam trong controller, giup code de bao tri, de test, va de bam use case hon

### 3. Coordinator / Application Service

- Ap dung qua `UserManagementCoordinator`, `RoleManagementCoordinator`, `CategoryManagementCoordinator`, `ProductManagementCoordinator`, `TransferCoordinator`
- Ly do dung: coordinator dieu phoi luong xu ly cua use case va map sat voi communication diagram trong doc

### 4. Dependency Injection

- Ap dung trong `Program.cs` thong qua `AddScoped`, `AddSingleton`
- Ly do dung: giam coupling, de thay the implementation, va dung voi best practice cua ASP.NET Core

### 5. ViewModel Pattern

- Ap dung qua cac lop trong `ViewModels/*`
- Vi du: `ViewModels/Users/UserFormViewModel.cs`, `ViewModels/TransferNotes/*`
- Ly do dung: du lieu UI duoc dong goi rieng, tranh dua truc tiep entity database len view

### 6. Facade-like Orchestration

- Ap dung ro nhat trong `TransferCoordinator`
- Ly do dung: module transfer note co nhieu participant; can mot diem dieu phoi trung tam de giu flow ro rang

### 7. Strategy theo trach nhiem nho

- Ap dung qua `FilterService`, `ValidationService`, `InventoryQueryService`, `InventoryService`
- Ly do dung: moi loai xu ly duoc tach rieng, de thay doi tung rule ma khong anh huong service khac

### 8. Repository / Unit of Work thong qua EF Core

- Ap dung qua `AppDbContext` va cac `DbSet`
- Ly do dung: khong can viet repository thu cong cho demo nay; EF Core da cung cap cach truy cap du lieu gon va nhat quan

### 9. Notification Pattern

- Ap dung qua `NotificationService` + `TempData`
- Ly do dung: hien thi message sau redirect cho create, update, approve, confirm out; phu hop voi flow web MVC

## Transfer Note Flow

Module transfer note hien bam theo cac UC sau:

- `UC-41` Create Transfer Note
- `UC-42` View Transfer Note
- `UC-43` Update Transfer Note
- `UC-44` Approve Transfer Note
- `UC-45` Confirm Transfer Out

Flow chinh:

`TransferInteraction -> TransferNotesController -> TransferCoordinator -> FilterService / ValidationService / LocationService / ProductService / InventoryQueryService / InventoryService / TransferService / AuditLogService`

## Cau truc thu muc

```text
Code/
|-- README.md
|-- docs/
|   `-- USECASE_MAPPING.md
`-- SupperChainErpDemo.Web/
    |-- Controllers/
    |-- Data/
    |-- Models/
    |-- Services/
    |-- ViewModels/
    |-- Views/
    |-- wwwroot/
    `-- Program.cs
```

## Chay local

```powershell
cd d:\Desktop\SWD392\Code\SupperChainErpDemo.Web
dotnet build
dotnet run --urls http://127.0.0.1:5055
```

## Tai khoan demo

- `admin.erp / Demo@123`
- `thu.merch / Demo@123`
- `huy.warehouse / Demo@123`
- `mai.chain / Demo@123`

## Tai lieu mapping

Chi tiet mapping use case sang code nam o:

- `docs/USECASE_MAPPING.md`

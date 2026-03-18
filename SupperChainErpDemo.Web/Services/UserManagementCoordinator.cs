using SupperChainErpDemo.Web.Models;
using SupperChainErpDemo.Web.ViewModels.Users;

namespace SupperChainErpDemo.Web.Services;

public class UserManagementCoordinator : IUserManagementCoordinator
{
    private readonly IUserService _userService;

    public UserManagementCoordinator(IUserService userService)
    {
        _userService = userService;
    }

    public UserIndexViewModel ShowUserList(string? keyword = null, string? statusFilter = null) =>
        _userService.BuildIndex(keyword, statusFilter);

    public UserDetailsViewModel? ShowUserDetails(string id) =>
        _userService.BuildDetails(id);

    public UserFormViewModel PrepareCreateUser() =>
        _userService.BuildCreateForm();

    public UserFormViewModel? PrepareUpdateUser(string id) =>
        _userService.BuildEditForm(id);

    public ServiceResult CreateUser(UserFormViewModel model) =>
        _userService.Create(model);

    public ServiceResult UpdateUser(string id, UserFormViewModel model) =>
        _userService.Update(id, model);

    public ServiceResult UpdateUserStatus(string id, RecordStatus status) =>
        _userService.ChangeStatus(id, status);
}

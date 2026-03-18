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
        _userService.GetUserList(keyword, statusFilter);

    public UserDetailsViewModel? ShowUserDetails(string id) =>
        _userService.GetUserDetails(id);

    public UserFormViewModel PrepareCreateUser() =>
        _userService.PrepareCreateUser();

    public UserFormViewModel? PrepareUpdateUser(string id) =>
        _userService.PrepareUpdateUser(id);

    public ServiceResult CreateUser(UserFormViewModel model) =>
        _userService.CreateUser(model);

    public ServiceResult UpdateUser(string id, UserFormViewModel model) =>
        _userService.UpdateUser(id, model);

    public ServiceResult UpdateUserStatus(string id, RecordStatus status) =>
        _userService.UpdateUserStatus(id, status);
}

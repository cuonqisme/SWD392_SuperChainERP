using SupperChainErpDemo.Web.Models;
using SupperChainErpDemo.Web.ViewModels.Users;

namespace SupperChainErpDemo.Web.Services;

public interface IUserManagementCoordinator
{
    UserIndexViewModel ShowUserList(string? keyword = null, string? statusFilter = null);

    UserDetailsViewModel? ShowUserDetails(string id);

    UserFormViewModel PrepareCreateUser();

    UserFormViewModel? PrepareUpdateUser(string id);

    ServiceResult CreateUser(UserFormViewModel model);

    ServiceResult UpdateUser(string id, UserFormViewModel model);

    ServiceResult UpdateUserStatus(string id, RecordStatus status);
}

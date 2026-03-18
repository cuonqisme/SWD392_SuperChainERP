using SupperChainErpDemo.Web.Models;
using SupperChainErpDemo.Web.ViewModels.Users;

namespace SupperChainErpDemo.Web.Services;

public interface IUserService
{
    UserIndexViewModel GetUserList(string? keyword = null, string? statusFilter = null);

    UserDetailsViewModel? GetUserDetails(string id);

    UserFormViewModel PrepareCreateUser();

    UserFormViewModel? PrepareUpdateUser(string id);

    ServiceResult CreateUser(UserFormViewModel model);

    ServiceResult UpdateUser(string id, UserFormViewModel model);

    ServiceResult UpdateUserStatus(string id, RecordStatus status);
}

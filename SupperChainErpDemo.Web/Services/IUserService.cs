using SupperChainErpDemo.Web.Models;
using SupperChainErpDemo.Web.ViewModels.Users;

namespace SupperChainErpDemo.Web.Services;

public interface IUserService
{
    IReadOnlyList<UserAccount> GetAll(string? keyword = null, string? statusFilter = null);

    UserAccount? GetById(string id);

    ServiceResult Create(UserFormViewModel model);

    ServiceResult Update(string id, UserFormViewModel model);

    ServiceResult ChangeStatus(string id, RecordStatus status);
}

using SupperChainErpDemo.Web.Models;
using SupperChainErpDemo.Web.ViewModels.Users;

namespace SupperChainErpDemo.Web.Services;

public interface IUserService
{
    UserIndexViewModel BuildIndex(string? keyword = null, string? statusFilter = null);

    UserDetailsViewModel? BuildDetails(string id);

    UserFormViewModel BuildCreateForm();

    UserFormViewModel? BuildEditForm(string id);

    ServiceResult Create(UserFormViewModel model);

    ServiceResult Update(string id, UserFormViewModel model);

    ServiceResult ChangeStatus(string id, RecordStatus status);
}

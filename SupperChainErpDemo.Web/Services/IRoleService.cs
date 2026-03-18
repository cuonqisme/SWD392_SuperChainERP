using SupperChainErpDemo.Web.Models;
using SupperChainErpDemo.Web.ViewModels.Roles;

namespace SupperChainErpDemo.Web.Services;

public interface IRoleService
{
    RoleIndexViewModel BuildIndex(string? statusFilter = null);

    Role? GetById(string id);

    RoleFormViewModel BuildCreateForm();

    RoleFormViewModel? BuildEditForm(string id);

    ServiceResult Create(RoleFormViewModel model);

    ServiceResult Update(string id, RoleFormViewModel model);

    ServiceResult Deactivate(string id);
}

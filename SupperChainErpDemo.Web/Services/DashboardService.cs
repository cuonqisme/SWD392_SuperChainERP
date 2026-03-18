using SupperChainErpDemo.Web.Models;
using SupperChainErpDemo.Web.ViewModels.Home;
using SupperChainErpDemo.Web.ViewModels.Shared;

namespace SupperChainErpDemo.Web.Services;

public class DashboardService : IDashboardService
{
    private readonly DemoDataStore _dataStore;

    public DashboardService(DemoDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public DashboardViewModel Build()
    {
        return new DashboardViewModel
        {
            StatCards =
            [
                new StatCardViewModel("Active Users", _dataStore.Users.Count(user => user.Status == RecordStatus.Active).ToString(), "UC-01 to UC-04"),
                new StatCardViewModel("Active Roles", _dataStore.Roles.Count(role => role.Status == RecordStatus.Active).ToString(), "UC-05 to UC-08"),
                new StatCardViewModel("Live Categories", _dataStore.Categories.Count(category => category.Status == RecordStatus.Active).ToString(), "UC-13 to UC-16"),
                new StatCardViewModel("Sale-ready Products", _dataStore.Products.Count(product => product.Status == RecordStatus.Active).ToString(), "UC-09 to UC-12")
            ],
            UseCaseFlows =
            [
                "Create -> validate master data -> persist -> notify actor",
                "View list -> inspect detail -> branch to update or deactivate",
                "Update -> validate dependencies -> save latest state",
                "Status change -> enforce business rules -> return success or warning"
            ],
            RecentHighlights =
            [
                $"{_dataStore.Users.OrderByDescending(user => user.UpdatedDate).First().FullName} is the latest updated user.",
                $"{_dataStore.Roles.OrderByDescending(role => role.UpdatedDate).First().RoleName} is the latest updated role.",
                $"{_dataStore.Products.OrderByDescending(product => product.UpdatedDate).First().ProductName} is the latest updated product."
            ]
        };
    }
}

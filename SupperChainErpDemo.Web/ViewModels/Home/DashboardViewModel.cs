using SupperChainErpDemo.Web.ViewModels.Shared;

namespace SupperChainErpDemo.Web.ViewModels.Home;

public class DashboardViewModel
{
    public IReadOnlyList<StatCardViewModel> StatCards { get; init; } = [];
}

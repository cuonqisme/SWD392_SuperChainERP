using SupperChainErpDemo.Web.ViewModels.Shared;

namespace SupperChainErpDemo.Web.ViewModels.Home;

public class DashboardViewModel
{
    public IReadOnlyList<StatCardViewModel> StatCards { get; init; } = [];

    public IReadOnlyList<string> UseCaseFlows { get; init; } = [];

    public IReadOnlyList<string> RecentHighlights { get; init; } = [];
}

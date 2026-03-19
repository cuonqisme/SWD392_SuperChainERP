using Microsoft.EntityFrameworkCore;
using SupperChainErpDemo.Web.Data;
using SupperChainErpDemo.Web.Models;
using SupperChainErpDemo.Web.ViewModels.Home;
using SupperChainErpDemo.Web.ViewModels.Shared;

namespace SupperChainErpDemo.Web.Services;

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _dbContext;

    public DashboardService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public DashboardViewModel Build()
    {
        var users = _dbContext.Users.AsNoTracking().ToList();
        var roles = _dbContext.Roles.AsNoTracking().ToList();
        var categories = _dbContext.Categories.AsNoTracking().ToList();
        var products = _dbContext.Products.AsNoTracking().ToList();
        var transferNotes = _dbContext.TransferNotes.AsNoTracking().ToList();

        return new DashboardViewModel
        {
            StatCards =
            [
                new StatCardViewModel("Active Users", users.Count(user => user.Status == RecordStatus.Active).ToString(), "UC-01 to UC-04"),
                new StatCardViewModel("Active Roles", roles.Count(role => role.Status == RecordStatus.Active).ToString(), "UC-05 to UC-08"),
                new StatCardViewModel("Live Categories", categories.Count(category => category.Status == RecordStatus.Active).ToString(), "UC-13 to UC-16"),
                new StatCardViewModel("Sale-ready Products", products.Count(product => product.Status == RecordStatus.Active).ToString(), "UC-09 to UC-12"),
                new StatCardViewModel("Open Transfers", transferNotes.Count(note => note.Status == TransferNoteStatus.Draft || note.Status == TransferNoteStatus.Approved).ToString(), "UC-41 to UC-45")
            ]
        };
    }
}

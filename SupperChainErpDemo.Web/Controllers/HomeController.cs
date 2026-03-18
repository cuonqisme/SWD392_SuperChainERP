using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SupperChainErpDemo.Web.Models;
using SupperChainErpDemo.Web.Services;

namespace SupperChainErpDemo.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IDashboardService _dashboardService;

    public HomeController(ILogger<HomeController> logger, IDashboardService dashboardService)
    {
        _logger = logger;
        _dashboardService = dashboardService;
    }

    public IActionResult Index()
    {
        return View("Dashboard", _dashboardService.Build());
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

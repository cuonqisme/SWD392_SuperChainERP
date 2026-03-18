using Microsoft.AspNetCore.Mvc;
using SupperChainErpDemo.Web.Services;
using SupperChainErpDemo.Web.ViewModels.TransferNotes;

namespace SupperChainErpDemo.Web.Controllers;

public class TransferNotesController : Controller
{
    private readonly ITransferCoordinator _transferCoordinator;
    private readonly INotificationService _notificationService;

    public TransferNotesController(
        ITransferCoordinator transferCoordinator,
        INotificationService notificationService)
    {
        _transferCoordinator = transferCoordinator;
        _notificationService = notificationService;
    }

    public IActionResult Index(
        string? searchTerm,
        string? statusFilter,
        string? sourceFilter,
        string? destinationFilter,
        DateTime? createdFrom,
        DateTime? createdTo)
    {
        return View(_transferCoordinator.ShowTransferNoteList(
            searchTerm,
            statusFilter,
            sourceFilter,
            destinationFilter,
            createdFrom,
            createdTo));
    }

    public IActionResult Details(string id)
    {
        var viewModel = _transferCoordinator.ShowTransferNoteDetails(id);
        if (viewModel is null)
        {
            _notificationService.Error("Transfer note not found", "The requested transfer note does not exist anymore.");
            return RedirectToAction(nameof(Index));
        }

        return View(viewModel);
    }

    public IActionResult Create() => View(_transferCoordinator.InitializeTransferDraft());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(TransferNoteFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model = _transferCoordinator.HydrateTransferDraft(model);
            return View(model);
        }

        var result = _transferCoordinator.CreateTransferNote(model);
        if (!result.Succeeded)
        {
            model = _transferCoordinator.HydrateTransferDraft(model);
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        _notificationService.Success("Transfer note created", result.Message);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(string id)
    {
        var viewModel = _transferCoordinator.PrepareTransferUpdate(id);
        if (viewModel is null)
        {
            _notificationService.Error("Transfer note unavailable", "The requested draft transfer note was not found or is no longer editable.");
            return RedirectToAction(nameof(Index));
        }

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(string id, TransferNoteFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model = _transferCoordinator.HydrateTransferUpdate(id, model);
            return View(model);
        }

        var result = _transferCoordinator.UpdateTransferNote(id, model);
        if (!result.Succeeded)
        {
            model = _transferCoordinator.HydrateTransferUpdate(id, model);
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        _notificationService.Success("Transfer note updated", result.Message);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Approve(string id)
    {
        var result = _transferCoordinator.ApproveTransferNote(id);
        if (!result.Succeeded)
        {
            _notificationService.Error("Transfer approval blocked", result.Message);
            return RedirectToAction(nameof(Index));
        }

        _notificationService.Success("Transfer note approved", result.Message);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Reject(string id, string? reason)
    {
        var result = _transferCoordinator.RejectTransferNote(id, reason);
        if (!result.Succeeded)
        {
            _notificationService.Error("Transfer rejection blocked", result.Message);
            return RedirectToAction(nameof(Index));
        }

        _notificationService.Success("Transfer note rejected", result.Message);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ConfirmTransferOut(string id)
    {
        var result = _transferCoordinator.ConfirmTransferOut(id);
        if (!result.Succeeded)
        {
            _notificationService.Error("Transfer-out blocked", result.Message);
            return RedirectToAction(nameof(Index));
        }

        _notificationService.Success("Transfer-out confirmed", result.Message);
        return RedirectToAction(nameof(Details), new { id });
    }
}

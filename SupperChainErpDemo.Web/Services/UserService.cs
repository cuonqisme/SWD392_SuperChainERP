using SupperChainErpDemo.Web.Models;
using SupperChainErpDemo.Web.ViewModels.Users;

namespace SupperChainErpDemo.Web.Services;

public class UserService : IUserService
{
    private readonly DemoDataStore _dataStore;

    public UserService(DemoDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public IReadOnlyList<UserAccount> GetAll(string? keyword = null, string? statusFilter = null)
    {
        var query = _dataStore.Users.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(user =>
                user.FullName.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                user.Username.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                user.Email.Contains(keyword, StringComparison.OrdinalIgnoreCase));
        }

        if (Enum.TryParse<RecordStatus>(statusFilter, true, out var status))
        {
            query = query.Where(user => user.Status == status);
        }

        return query
            .OrderByDescending(user => user.UpdatedDate)
            .ThenBy(user => user.FullName)
            .ToList();
    }

    public UserAccount? GetById(string id) =>
        _dataStore.Users.FirstOrDefault(user => user.UserId.Equals(id, StringComparison.OrdinalIgnoreCase));

    public ServiceResult Create(UserFormViewModel model)
    {
        var validationError = Validate(model);
        if (validationError is not null)
        {
            return ServiceResult.Failure(validationError);
        }

        var user = new UserAccount
        {
            UserId = _dataStore.NextUserId(),
            RoleId = model.RoleId,
            Username = model.Username.Trim(),
            PasswordHash = DemoDataStore.HashPassword(model.Password.Trim()),
            FullName = model.FullName.Trim(),
            Email = model.Email.Trim(),
            Phone = model.Phone.Trim(),
            Status = RecordStatus.Active,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow
        };

        _dataStore.Users.Add(user);
        return ServiceResult.Success($"User {user.FullName} was created and assigned to the selected role.");
    }

    public ServiceResult Update(string id, UserFormViewModel model)
    {
        var user = GetById(id);
        if (user is null)
        {
            return ServiceResult.Failure("The selected user could not be found.");
        }

        var validationError = Validate(model, id);
        if (validationError is not null)
        {
            return ServiceResult.Failure(validationError);
        }

        user.RoleId = model.RoleId;
        user.Username = model.Username.Trim();
        user.FullName = model.FullName.Trim();
        user.Email = model.Email.Trim();
        user.Phone = model.Phone.Trim();
        user.UpdatedDate = DateTime.UtcNow;

        if (!string.IsNullOrWhiteSpace(model.Password))
        {
            user.PasswordHash = DemoDataStore.HashPassword(model.Password.Trim());
        }

        return ServiceResult.Success($"User {user.FullName} was updated successfully.");
    }

    public ServiceResult ChangeStatus(string id, RecordStatus status)
    {
        var user = GetById(id);
        if (user is null)
        {
            return ServiceResult.Failure("The selected user could not be found.");
        }

        var role = _dataStore.Roles.FirstOrDefault(item => item.RoleId == user.RoleId);
        if (status == RecordStatus.Active && role?.Status != RecordStatus.Active)
        {
            return ServiceResult.Failure("Users can only be activated when their assigned role is active.");
        }

        user.Status = status;
        user.UpdatedDate = DateTime.UtcNow;
        return ServiceResult.Success($"User {user.FullName} status changed to {status}.");
    }

    private string? Validate(UserFormViewModel model, string? currentId = null)
    {
        var role = _dataStore.Roles.FirstOrDefault(item => item.RoleId == model.RoleId);
        if (role is null)
        {
            return "Please choose a valid role.";
        }

        if (role.Status != RecordStatus.Active)
        {
            return "Users can only be created or updated with an active role.";
        }

        if (string.IsNullOrWhiteSpace(model.Username) ||
            string.IsNullOrWhiteSpace(model.Email) ||
            string.IsNullOrWhiteSpace(model.FullName))
        {
            return "Username, full name, and email are required.";
        }

        if (string.IsNullOrWhiteSpace(currentId) && string.IsNullOrWhiteSpace(model.Password))
        {
            return "A temporary password is required when creating a user.";
        }

        var username = model.Username.Trim();
        var email = model.Email.Trim();

        var duplicateUsername = _dataStore.Users.Any(user =>
            !user.UserId.Equals(currentId, StringComparison.OrdinalIgnoreCase) &&
            user.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

        if (duplicateUsername)
        {
            return "Username already exists.";
        }

        var duplicateEmail = _dataStore.Users.Any(user =>
            !user.UserId.Equals(currentId, StringComparison.OrdinalIgnoreCase) &&
            user.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

        return duplicateEmail ? "Email already exists." : null;
    }
}

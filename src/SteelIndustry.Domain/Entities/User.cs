using SteelIndustry.Domain.Enums;

namespace SteelIndustry.Domain.Entities;

public class User
{
    public string Id { get; private set; }
    public string Username { get; private set; }
    public string PasswordHash { get; private set; }
    public string FullName { get; private set; }
    public string Email { get; private set; }
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public User(string id, string username, string passwordHash, string fullName, string email, UserRole role)
    {
        Id = string.IsNullOrWhiteSpace(id) ? Guid.NewGuid().ToString() : id;
        Username = username ?? throw new ArgumentNullException(nameof(username));
        PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
        FullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Role = role;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateProfile(string fullName, string email, UserRole role)
    {
        FullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Role = role;
    }

    public void SetActive(bool isActive)
    {
        IsActive = isActive;
    }

    public void ChangePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash ?? throw new ArgumentNullException(nameof(newPasswordHash));
    }
}

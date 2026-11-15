using System.Collections.Concurrent;
using SteelIndustry.Application.Interfaces.Security;
using SteelIndustry.Domain.Entities;
using SteelIndustry.Domain.Enums;
using SteelIndustry.Domain.Repositories;

namespace SteelIndustry.Infrastructure.Repositories;

public class InMemoryUserRepository : IUserRepository
{
    private static readonly ConcurrentDictionary<string, User> _users = new();

    public InMemoryUserRepository(IPasswordHasher passwordHasher)
    {
        if (_users.IsEmpty)
        {
            SeedUsers(passwordHasher);
        }
    }

    private static void SeedUsers(IPasswordHasher passwordHasher)
    {
        var admin = new User(
            id: "usr-admin-01",
            username: "admin",
            passwordHash: passwordHasher.HashPassword("Admin@123"),
            fullName: "Administrador do Sistema",
            email: "admin@siderurgia.ind.br",
            role: UserRole.Admin
        );

        var engenheiro = new User(
            id: "usr-eng-02",
            username: "engenheiro.silva",
            passwordHash: passwordHasher.HashPassword("Engenheiro@123"),
            fullName: "Engenheiro Carlos Silva (Metalurgia)",
            email: "carlos.silva@siderurgia.ind.br",
            role: UserRole.EngenheiroMetalurgista
        );

        var operador = new User(
            id: "usr-op-03",
            username: "operador.aciaria",
            passwordHash: passwordHasher.HashPassword("Operador@123"),
            fullName: "Marcos Lima (Operador Aciaria)",
            email: "marcos.lima@siderurgia.ind.br",
            role: UserRole.Operador
        );

        _users.TryAdd(admin.Id, admin);
        _users.TryAdd(engenheiro.Id, engenheiro);
        _users.TryAdd(operador.Id, operador);
    }

    public Task<User?> GetByIdAsync(string id)
    {
        _users.TryGetValue(id, out var user);
        return Task.FromResult(user);
    }

    public Task<User?> GetByUsernameAsync(string username)
    {
        var user = _users.Values.FirstOrDefault(u => 
            string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(user);
    }

    public Task<User[]> GetAllAsync()
    {
        return Task.FromResult(_users.Values.ToArray());
    }

    public Task AddAsync(User user)
    {
        _users.TryAdd(user.Id, user);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(User user)
    {
        _users[user.Id] = user;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(string id)
    {
        _users.TryRemove(id, out _);
        return Task.CompletedTask;
    }
}

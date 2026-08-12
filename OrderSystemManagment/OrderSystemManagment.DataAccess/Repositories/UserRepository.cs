using Microsoft.EntityFrameworkCore;
using OrderSystemManagment.Domain.Entities;
using OrderSystemManagment.Domain.Entities.BaseModels;
using OrderSystemManagment.Domain.Interfaces;
using OrderSystemManagment.DataAccess.Context;

namespace OrderSystemManagment.DataAccess.Repositories;

public class UserRepository : IUserRepository
{
    private readonly OrderDbContext _context;

    public UserRepository(OrderDbContext context)
    {
        _context = context;
    }

    private void EnforceAdminPermissions(Role? role)
    {
        if (role is not null)
        {
            role.OrderManagement.IsUserAdmin(role);
            role.UserManagement.IsUserAdmin(role);
        }
    }

    public async Task<User?> GetUserByCredentialsAsync(string userName, string password)
    {
        var user = await _context.Users
            .AsNoTracking()
            .Include(x => x.Role)
            .FirstOrDefaultAsync(x => x.UserName == userName && x.Password == password);
        
        if (user?.Role is not null)
        {
            EnforceAdminPermissions(user.Role);
        }
        
        return user;
    }

    public async Task<IEnumerable<User>> GetUsersAsync()
    {
        var users = await _context.Users
            .AsNoTracking()
            .Include(x => x.Role)
            .OrderByDescending(x => x.Id)
            .ToListAsync();
            
        foreach (var user in users)
        {
            if (user.Role is not null)
            {
                EnforceAdminPermissions(user.Role);
            }
        }
        
        return users;
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        var user = await _context.Users
            .AsNoTracking()
            .Include(x => x.Role)
            .FirstOrDefaultAsync(x => x.Id == id);
            
        if (user?.Role is not null)
        {
            EnforceAdminPermissions(user.Role);
        }
        
        return user;
    }

    public async Task SaveUserAsync(User user)
    {
        if (user.Role is not null)
            user.Role = AttachOrGetTracked(user.Role);

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateUserAsync(User user)
    {
        var existing = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == user.Id);

        if (existing is null)
            throw new InvalidOperationException($"User with id {user.Id} was not found.");

        existing.UserName = user.UserName;
        existing.Password = user.Password;

        if (user.Role is not null)
            existing.Role = AttachOrGetTracked(user.Role);

        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteUserAsync(int id)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == id);

        if (user is null) return;

        user.IsDeleted = true;
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Role>> GetRolesAsync()
    {
        var roles = await _context.Roles
            .AsNoTracking()
            .ToListAsync();
            
        foreach (var role in roles)
        {
            EnforceAdminPermissions(role);
        }
        
        return roles;
    }

    public async Task<Role?> GetRoleByIdAsync(int id)
    {
        var role = await _context.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
            
        if (role is not null)
        {
            EnforceAdminPermissions(role);
        }
        
        return role;
    }

    public async Task SaveRoleAsync(Role role)
    {
        await _context.Roles.AddAsync(role);
        await _context.SaveChangesAsync();
    }

    private TEntity AttachOrGetTracked<TEntity>(TEntity entity) where TEntity : DatabaseObject
    {
        if (entity == null) return null!;

        var tracked = _context.Set<TEntity>().Local.FirstOrDefault(x => x.Id == entity.Id);
        if (tracked != null)
            return tracked;

        _context.Attach(entity);
        return entity;
    }
}

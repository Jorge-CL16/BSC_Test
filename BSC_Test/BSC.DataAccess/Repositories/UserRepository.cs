using BSC.DataAccess.Context;
using BSC.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace BSC.DataAccess.Repositories;

/*
 Implementación del repositorio de usuarios
 */
public sealed class UserRepository(BscDbContext dbContext) : IUserRepository
{
    /*Busca un usuario por id*/
    public async Task<User?> GetByIdAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Users
            .FirstOrDefaultAsync(user => user.UserId == userId, cancellationToken);
    }

    /*Buscar usuario por correo*/
    public async Task<User?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Users
            .FirstOrDefaultAsync(user => user.Email == email, cancellationToken);
    }

    /*Consultar todos los usuarios ordenados por nombre*/
    public async Task<IReadOnlyList<User>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Users
            .AsNoTracking()
            .OrderBy(user => user.Name)
            .ToListAsync(cancellationToken);
    }

    /*Insertar un nuevo usuario y guarda los cambios*/
    public async Task<User> AddAsync(
        User user,
        CancellationToken cancellationToken = default)
    {
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);
        return user;
    }

    /*Actualizar la entidad del usuario en la base de datos*/
    public async Task UpdateAsync(
        User user,
        CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

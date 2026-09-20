using BSC.DataAccess.Entities;

namespace BSC.DataAccess.Repositories;

/*
 Contrato del repositorio para operaciones de consulta y persistencia de cuentas de usuarios
 */
public interface IUserRepository
{
    /*Obtiener usuario por su identificador*/
    Task<User?> GetByIdAsync(
        int userId,
        CancellationToken cancellationToken = default);

    /*Obtener un usuario por su dirección de correo*/
    Task<User?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    /*Obtener el catálogo completo de uusuarios registrados en el sistema*/
    Task<IReadOnlyList<User>> GetAllAsync(
        CancellationToken cancellationToken = default);

    /*Registra un nuevo usuario en la base de datos de manera asíncrona*/
    Task<User> AddAsync(
        User user,
        CancellationToken cancellationToken = default);

    /*Actualiza los campos modificados de un usuario en la base de datos*/
    Task UpdateAsync(
        User user,
        CancellationToken cancellationToken = default);
}

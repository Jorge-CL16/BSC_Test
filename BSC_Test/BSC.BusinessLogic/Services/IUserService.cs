using BSC.BusinessLogic.Models;

namespace BSC.BusinessLogic.Services;

/*
 Contrato del servicio de negocio para administración de usuarios, autenticación y contraseñas.
 */
public interface IUserService
{
    /* Obtiene el catálogo completo de cuentas de usuarios registradas.*/
    Task<IReadOnlyList<UserDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    /*Validar reglas de contraseña y crea una nueva cuenta de usuario con hash*/
    Task<UserDto> CreateAsync(
        CreateUserRequest request,
        CancellationToken cancellationToken = default);

    /*Autentica las credenciales de correo y contraseña contra el hash almacenado*/
    Task<UserDto?> AuthenticateAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default);

    /*Habilita o deshabilita una cuenta*/
    Task<UserDto?> SetActiveAsync(
        int userId,
        bool active,
        CancellationToken cancellationToken = default);
}

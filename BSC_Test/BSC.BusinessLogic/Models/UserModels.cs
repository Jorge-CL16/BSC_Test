namespace BSC.BusinessLogic.Models;

/*
 Roles de usuario con permisos específicos dentro de la plataforma BSC:
 1 = Administrator (Control total).
 2 = AdministrativeStaff (Gestión de productos, existencias y cancelación).
 3 = Salesperson (Consulta de catálogo y creación de pedidos).
 */

/*Este enum para no generar una tabla extra ademas, esto es para no crear una tabla (ademas se puede crear helper en caso de una descripcion)*/
public enum UserRole : byte
{
    Administrator = 1,
    AdministrativeStaff = 2,
    Salesperson = 3
}

/* Solicitud para registrar una nueva cuenta de usuario en la plataforma */
public sealed record CreateUserRequest(
    string Name,
    string Email,
    string Password,
    UserRole UserRole);

/* Solicitud de autenticación inicio de sesión */
public sealed record LoginRequest(
    string Email,
    string Password);

/* Objeto de transferencia con los datos públicos de una cuenta de usuari */
public sealed record UserDto(
    int UserId,
    string Name,
    string Email,
    UserRole UserRole,
    bool Active);

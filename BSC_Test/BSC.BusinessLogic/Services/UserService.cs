using BSC.BusinessLogic.Models;
using BSC.DataAccess.Entities;
using BSC.DataAccess.Repositories;
using Microsoft.AspNetCore.Identity;

namespace BSC.BusinessLogic.Services;

/*
 Implementación del servicio de negocio para usuarios, verificación de contraseñas y autenticación.
 */
public sealed class UserService(
    IUserRepository userRepository,
    IPasswordHasher<User> passwordHasher) : IUserService
{
    /*Obtiene todos los usuari*/
    public async Task<IReadOnlyList<UserDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var users = await userRepository.GetAllAsync(cancellationToken);
        return users.Select(Map).ToList();
    }

    /*Valida y crea una cuenta de usuario con hash*/
    public async Task<UserDto> CreateAsync(
        CreateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        var name = request.Name?.Trim();
        var email = request.Email?.Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre de usuario es obligatorio.", nameof(request));

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new ArgumentException("Se requiere un correo electrónico válido.", nameof(request));

        if (!Enum.IsDefined(request.UserRole))
            throw new ArgumentException("El rol de usuario no es válido.", nameof(request));

        ValidatePassword(request.Password);

        if (await userRepository.GetByEmailAsync(email, cancellationToken) is not null)
            throw new ArgumentException("El correo electrónico ya está registrado.", nameof(request));

        var user = new User
        {
            UserRoleId = (byte)request.UserRole,
            Name = name,
            Email = email,
            Active = true
        };

        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

        var createdUser = await userRepository.AddAsync(user, cancellationToken);

        return new UserDto(
            createdUser.UserId,
            createdUser.Name,
            createdUser.Email,
            (UserRole)createdUser.UserRoleId,
            createdUser.Active);
    }

    /*Autentica credenciales verificando el hash de la contraseña*/
    public async Task<UserDto?> AuthenticateAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var email = request.Email?.Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrEmpty(request.Password))
            return null;

        var user = await userRepository.GetByEmailAsync(email, cancellationToken);

        if (user is null || !user.Active)
            return null;

        var passwordResult = passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            request.Password);

        if (passwordResult == PasswordVerificationResult.Failed)
            return null;

        return new UserDto(
            user.UserId,
            user.Name,
            user.Email,
            (UserRole)user.UserRoleId,
            user.Active);
    }

    /*Activa o desactiva una cuenta*/
    public async Task<UserDto?> SetActiveAsync(
        int userId,
        bool active,
        CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(userId, cancellationToken);

        if (user is null)
            return null;

        user.Active = active;
        await userRepository.UpdateAsync(user, cancellationToken);
        return Map(user);
    }

    private static UserDto Map(User user)
    {
        return new UserDto(
            user.UserId,
            user.Name,
            user.Email,
            (UserRole)user.UserRoleId,
            user.Active);
    }

    private static void ValidatePassword(string? password)
    {
        if (string.IsNullOrEmpty(password)
            || password.Length < 8
            || !password.Any(char.IsLetter)
            || !password.Any(char.IsDigit))
        {
            throw new ArgumentException(
                "La contraseña debe contener al menos 8 caracteres, una letra y un número.",
                nameof(password));
        }
    }
}

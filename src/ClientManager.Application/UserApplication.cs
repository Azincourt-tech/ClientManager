using ClientManager.Application.Dtos.User;
using ClientManager.Application.Interfaces;
using ClientManager.Application.Mappers;
using ClientManager.Domain.Core.Responses;
using FluentValidation;

namespace ClientManager.Application;

public class UserApplication : IUserApplication
{
    private readonly IUserService _userService;
    private readonly IValidator<CreateUserDto> _createUserValidator;

    public UserApplication(IUserService userService, IValidator<CreateUserDto> createUserValidator)
    {
        _userService = userService;
        _createUserValidator = createUserValidator;
    }

    public async Task<ServiceResponse<UserDto>> CreateUserAsync(CreateUserDto createUserDto)
    {
        var validationResult = await _createUserValidator.ValidateAsync(createUserDto).ConfigureAwait(false);

        if (!validationResult.IsValid)
        {
            var firstError = validationResult.Errors.First().ErrorMessage;
            return ServiceResponse<UserDto>.Fail(firstError);
        }

        var existingUser = await _userService.GetUserByUsernameAsync(createUserDto.Username).ConfigureAwait(false);
        if (existingUser != null)
            return ServiceResponse<UserDto>.Fail("UsernameAlreadyExists");

        var existingEmail = await _userService.GetUserByEmailAsync(createUserDto.Email).ConfigureAwait(false);
        if (existingEmail != null)
            return ServiceResponse<UserDto>.Fail("EmailAlreadyExists");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);
        var user = createUserDto.ToModel(passwordHash);

        await _userService.AddUserAsync(user).ConfigureAwait(false);

        return ServiceResponse<UserDto>.Ok(user.ToDto());
    }

    public async Task<ServiceResponse<UserDto>> GetUserByIdAsync(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id).ConfigureAwait(false);

        if (user == null)
            return ServiceResponse<UserDto>.Fail("UserNotFound");

        return ServiceResponse<UserDto>.Ok(user.ToDto());
    }

    public async Task<ServiceResponse<IEnumerable<UserDto>>> GetUsersAsync()
    {
        var users = await _userService.GetUsersAsync().ConfigureAwait(false);
        var userDtos = users.Select(u => u.ToDto());
        return ServiceResponse<IEnumerable<UserDto>>.Ok(userDtos);
    }

    public async Task<ServiceResponse<UserDto>> UpdateUserAsync(Guid id, CreateUserDto updateUserDto)
    {
        var validationResult = await _createUserValidator.ValidateAsync(updateUserDto).ConfigureAwait(false);

        if (!validationResult.IsValid)
        {
            var firstError = validationResult.Errors.First().ErrorMessage;
            return ServiceResponse<UserDto>.Fail(firstError);
        }

        var existingUser = await _userService.GetUserByIdAsync(id).ConfigureAwait(false);

        if (existingUser == null)
            return ServiceResponse<UserDto>.Fail("UserNotFound");

        existingUser.UpdateDetails(updateUserDto.Username, updateUserDto.Email, updateUserDto.Role);

        if (!string.IsNullOrWhiteSpace(updateUserDto.Password))
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(updateUserDto.Password);
            existingUser.UpdatePasswordHash(passwordHash);
        }

        await _userService.UpdateUserAsync(existingUser).ConfigureAwait(false);

        return ServiceResponse<UserDto>.Ok(existingUser.ToDto());
    }

    public async Task<ServiceResponse<string>> DeleteUserAsync(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id).ConfigureAwait(false);

        if (user == null)
            return ServiceResponse<string>.Fail("UserNotFound");

        user.Deactivate();
        await _userService.UpdateUserAsync(user).ConfigureAwait(false);

        return ServiceResponse<string>.Ok(id.ToString(), "UserDeactivated");
    }
}

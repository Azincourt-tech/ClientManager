using ClientManager.Application.Mappers;
using ClientManager.Domain.Core.Responses;
using FluentValidation;

namespace ClientManager.Application
{
    public class UserApplication : IUserApplication
    {
        private readonly IUserService _userService;
        private readonly IValidator<Dtos.User.CreateUserDto> _createUserValidator;

        public UserApplication(IUserService userService, IValidator<Dtos.User.CreateUserDto> createUserValidator)
        {
            _userService = userService;
            _createUserValidator = createUserValidator;
        }

        public async Task<ServiceResponse<Guid>> AddUserAsync(Dtos.User.CreateUserDto userDto)
        {
            var validationResult = await _createUserValidator.ValidateAsync(userDto).ConfigureAwait(false);

            if (!validationResult.IsValid)
            {
                var firstError = validationResult.Errors.First().ErrorMessage;
                return ServiceResponse<Guid>.Fail(firstError);
            }

            var existingUser = await _userService.GetUserByUsernameAsync(userDto.Username).ConfigureAwait(false);
            if (existingUser != null)
                return ServiceResponse<Guid>.Fail("UsernameAlreadyExists");

            var existingEmail = await _userService.GetUserByEmailAsync(userDto.Email).ConfigureAwait(false);
            if (existingEmail != null)
                return ServiceResponse<Guid>.Fail("EmailAlreadyExists");

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
            var user = userDto.ToModel(passwordHash);

            await _userService.AddUserAsync(user).ConfigureAwait(false);
            return ServiceResponse<Guid>.Ok(user.Id, "UserInserted");
        }

        public async Task<ServiceResponse<string>> UpdateUserAsync(Guid id, Dtos.User.CreateUserDto userDto)
        {
            var validationResult = await _createUserValidator.ValidateAsync(userDto).ConfigureAwait(false);

            if (!validationResult.IsValid)
            {
                var firstError = validationResult.Errors.First().ErrorMessage;
                return ServiceResponse<string>.Fail(firstError);
            }

            var existingUser = await _userService.GetUserByIdAsync(id).ConfigureAwait(false);
            if (existingUser == null)
                return ServiceResponse<string>.Fail("UserNotFound");

            existingUser.UpdateDetails(userDto.Username, userDto.Email, userDto.Role);

            if (!string.IsNullOrWhiteSpace(userDto.Password))
            {
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
                existingUser.UpdatePasswordHash(passwordHash);
            }

            await _userService.UpdateUserAsync(existingUser).ConfigureAwait(false);
            return ServiceResponse<string>.Ok(existingUser.Id.ToString(), "UserUpdated");
        }

        public async Task<ServiceResponse<string>> DeleteUserByIdAsync(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id).ConfigureAwait(false);
            if (user == null)
                return ServiceResponse<string>.Fail("UserNotFound");

            await _userService.DeleteUserByIdAsync(id).ConfigureAwait(false);
            return ServiceResponse<string>.Ok(id.ToString(), "UserDeleted");
        }

        public async Task<ServiceResponse<IEnumerable<Dtos.User.UserDto>>> GetUsersAsync()
        {
            var users = await _userService.GetUsersAsync().ConfigureAwait(false);
            var usersDto = users.Select(u => u.ToDto());
            return ServiceResponse<IEnumerable<Dtos.User.UserDto>>.Ok(usersDto);
        }

        public async Task<ServiceResponse<Dtos.User.UserDto?>> GetUserByIdAsync(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id).ConfigureAwait(false);
            if (user == null)
                return ServiceResponse<Dtos.User.UserDto?>.Fail("UserNotFound");

            return ServiceResponse<Dtos.User.UserDto?>.Ok(user.ToDto());
        }
    }
}

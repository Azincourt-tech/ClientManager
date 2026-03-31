using ClientManager.Application.Dtos.User;
using ClientManager.Domain.Model;

namespace ClientManager.Application.Mappers
{
    public static class UserMapper
    {
        public static UserDto ToDto(this User user)
        {
            if (user == null) return null!;

            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };
        }

        public static User ToModel(this CreateUserDto dto, string passwordHash)
        {
            if (dto == null) return null!;

            return new User(
                dto.Username,
                dto.Email,
                passwordHash,
                dto.Role
            );
        }
    }
}

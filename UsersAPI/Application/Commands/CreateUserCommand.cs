using MediatR;
using UsersAPI.Application.DTOs;
namespace UsersAPI.Application.Commands
{
    public record CreateUserCommand(string FirstName, string LastName, string Email) : IRequest<UserDto>;
}

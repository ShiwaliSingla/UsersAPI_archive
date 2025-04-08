using MediatR;
using UsersAPI.Application.DTOs;

namespace UsersAPI.Application.Commands
{
    public record UpdateUserCommand(Guid Id, string FirstName, string LastName, string Email) : IRequest<UserDto>;
}

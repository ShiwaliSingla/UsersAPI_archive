using MediatR;
using UsersAPI.Application.DTOs;

namespace UsersAPI.Application.Queries
{
    public record GetUserQuery(Guid Id) : IRequest<UserDto>;
}

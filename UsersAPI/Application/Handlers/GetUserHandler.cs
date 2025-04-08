using MediatR;
using UsersAPI.Application.DTOs;
using UsersAPI.Application.Queries;
using UsersAPI.Domain.Interfaces;

namespace UsersAPI.Application.Handlers
{
    public class GetUserHandler : IRequestHandler<GetUserQuery, UserDto>
    {
        private readonly IUserRepository _repository;

        public GetUserHandler(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<UserDto> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _repository.GetByIdAsync(request.Id);
            return new UserDto(user.Id, user.FirstName, user.LastName, user.Email);
        }
    }
}

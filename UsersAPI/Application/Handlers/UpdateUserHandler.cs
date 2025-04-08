using MediatR;
using UsersAPI.Application.Commands;
using UsersAPI.Application.DTOs;
using UsersAPI.Domain.Interfaces;

namespace UsersAPI.Application.Handlers
{
    public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, UserDto>
    {
        private readonly IUserRepository _repository;

        public UpdateUserHandler(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _repository.GetByIdAsync(request.Id);

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;

            await _repository.UpdateAsync(user);

            return new UserDto(user.Id, user.FirstName, user.LastName, user.Email);
        }
    }
}

using MediatR;
using UsersAPI.Application.Commands;
using UsersAPI.Application.DTOs;
using UsersAPI.Domain.Entities;
using UsersAPI.Domain.Interfaces;

namespace UsersAPI.Application.Handlers
{
    public class CreateUserHandler : IRequestHandler<CreateUserCommand, UserDto>
    {
        private readonly IUserRepository _repository;
        private readonly ILogger<CreateUserHandler> _logger;

        public CreateUserHandler(IUserRepository repository, ILogger<CreateUserHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email
            };

            await _repository.CreateAsync(user);
            _logger.LogInformation("User created: {Email}", user.Email);

            return new UserDto(user.Id, user.FirstName, user.LastName, user.Email);
        }
    }
}

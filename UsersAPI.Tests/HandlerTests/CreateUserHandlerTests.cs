using Moq;
using Microsoft.Extensions.Logging;
using UsersAPI.Application.Commands;
using UsersAPI.Application.Handlers;
using UsersAPI.Domain.Interfaces;
using UsersAPI.Domain.Entities;

namespace UnitTests.HandlerTests
{
    public class CreateUserHandlerTests
    {
        private readonly Mock<IUserRepository> _repositoryMock;
        private readonly Mock<ILogger<CreateUserHandler>> _loggerMock;
        private readonly CreateUserHandler _handler;

        public CreateUserHandlerTests()
        {
            _repositoryMock = new Mock<IUserRepository>();
            _loggerMock = new Mock<ILogger<CreateUserHandler>>();
            _handler = new CreateUserHandler(_repositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldCreateUser_WhenRequestIsValid()
        {
            // Arrange
            var command = new CreateUserCommand("John", "Doe", "john.doe@example.com");

            _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask)
                .Callback<User>(user => user.Id = Guid.NewGuid());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(command.FirstName, result.FirstName);
            Assert.Equal(command.LastName, result.LastName);
            Assert.Equal(command.Email, result.Email);
            _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenRepositoryFails()
        {
            // Arrange
            var command = new CreateUserCommand("John", "Doe", "john.doe@example.com");

            _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<User>()))
                .ThrowsAsync(new Exception("Database failure"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldLogInformation_WhenUserIsCreated()
        {
            // Arrange
            var command = new CreateUserCommand("John", "Doe", "john.doe@example.com");

            _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("User created")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnCorrectUserDto()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new CreateUserCommand("John", "Doe", "john.doe@example.com");

            _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<User>()))
                .Callback<User>(u => u.Id = userId)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(userId, result.Id);
            Assert.Equal("John", result.FirstName);
            Assert.Equal("Doe", result.LastName);
            Assert.Equal("john.doe@example.com", result.Email);
        }
    }
}

using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersAPI.Application.Commands;
using UsersAPI.Application.DTOs;
using UsersAPI.Application.Handlers;
using UsersAPI.Domain.Entities;
using UsersAPI.Domain.Exceptions;
using UsersAPI.Domain.Interfaces;

namespace UnitTests.HandlerTests
{
    public class UpdateUserHandlerTests
    {
        private readonly Mock<IUserRepository> _repositoryMock;
        private readonly UpdateUserHandler _handler;

        public UpdateUserHandlerTests()
        {
            _repositoryMock = new Mock<IUserRepository>();
            _handler = new UpdateUserHandler(_repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldUpdateUser_WhenUserExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new UpdateUserCommand(userId, "John", "Doe", "john.doe@example.com");

            var existingUser = new User
            {
                Id = userId,
                FirstName = "Jane",
                LastName = "Doe",
                Email = "jane.doe@example.com"
            };

            var updatedUserDto = new UserDto(userId, "John", "Doe", "john.doe@example.com");

            _repositoryMock.Setup(r => r.GetByIdAsync(userId))
                           .ReturnsAsync(existingUser);

            _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<User>()))
                           .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(updatedUserDto.Id, result.Id);
            Assert.Equal(updatedUserDto.FirstName, result.FirstName);
            Assert.Equal(updatedUserDto.LastName, result.LastName);
            Assert.Equal(updatedUserDto.Email, result.Email);
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new UpdateUserCommand(userId, "John", "Doe", "john.doe@example.com");

            _repositoryMock.Setup(r => r.GetByIdAsync(userId))
                           .ThrowsAsync(new NotFoundException($"User with ID {userId} not found"));

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        }
    }

}

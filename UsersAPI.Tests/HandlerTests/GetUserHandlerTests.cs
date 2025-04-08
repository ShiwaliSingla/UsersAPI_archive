using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersAPI.Application.DTOs;
using UsersAPI.Application.Handlers;
using UsersAPI.Application.Queries;
using UsersAPI.Domain.Entities;
using UsersAPI.Domain.Exceptions;
using UsersAPI.Domain.Interfaces;

namespace UnitTests.HandlerTests
{
    public class GetUserHandlerTests
    {
        private readonly Mock<IUserRepository> _repositoryMock;
        private readonly GetUserHandler _handler;

        public GetUserHandlerTests()
        {
            _repositoryMock = new Mock<IUserRepository>();
            _handler = new GetUserHandler(_repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            };
            var userDto = new UserDto(user.Id, user.FirstName, user.LastName, user.Email);

            _repositoryMock.Setup(r => r.GetByIdAsync(userId))
                           .ReturnsAsync(user);

            var query = new GetUserQuery(userId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            Assert.Equal(user.FirstName, result.FirstName);
            Assert.Equal(user.LastName, result.LastName);
            Assert.Equal(user.Email, result.Email);
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.GetByIdAsync(userId))
                           .ThrowsAsync(new NotFoundException($"User with ID {userId} not found"));

            var query = new GetUserQuery(userId);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(query, CancellationToken.None));
        }
    }

}

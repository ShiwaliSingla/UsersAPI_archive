using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using UsersAPI.Application.Commands;
using UsersAPI.Application.DTOs;
using UsersAPI.Application.Queries;
using UsersAPI.Controllers;
using UsersAPI.Domain.Exceptions;

namespace UnitTests.ControllerTests
{
    public class UserControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new UserController(_mediatorMock.Object);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WhenSuccessful()
        {
            // Arrange
            var command = new CreateUserCommand("John", "Doe", "john.doe@example.com");

            var createdUser = new UserDto(Guid.NewGuid(), command.FirstName, command.LastName, command.Email);

            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(createdUser);

            // Act
            var result = await _controller.Create(command);

            // Assert
            var actionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<UserDto>(actionResult.Value);
            Assert.Equal(command.Email, returnValue.Email);
        }

        [Fact]
        public async Task Create_Returns500_WhenMediatorThrows()
        {
            // Arrange
            var command = new CreateUserCommand("John", "Doe", "john.doe@example.com");

            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new Exception("Mediator failed"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.Create(command));
        }
        [Fact]
        public async Task Get_ReturnsOk_WhenUserFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userDto = new UserDto(userId, "John", "Doe", "john.doe@example.com");

            _mediatorMock.Setup(m => m.Send(It.Is<GetUserQuery>(q => q.Id == userId), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(userDto);

            // Act
            var result = await _controller.Get(userId);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<UserDto>(actionResult.Value);
            Assert.Equal(userDto.Email, returnValue.Email);
            Assert.Equal(userDto.FirstName, returnValue.FirstName);
        }

        [Fact]
        public async Task Get_ReturnsNotFound_WhenUserNotFound_SecondVariation()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _mediatorMock.Setup(m => m.Send(It.Is<GetUserQuery>(q => q.Id == userId), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new NotFoundException($"User with ID {userId} not found"));

            // Act
            var result = await _controller.GetWithExceptionHandling(userId, true);

            // Assert
            var actionResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        }
        [Fact]
        public async Task Update_ReturnsOk_WhenUserUpdated()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new UpdateUserCommand(userId, "John", "Doe", "john.doe@example.com");
            var updatedUserDto = new UserDto(userId, "John", "Doe", "john.doe@example.com");

            _mediatorMock.Setup(m => m.Send(It.Is<UpdateUserCommand>(c => c.Id == userId), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(updatedUserDto);

            // Act
            var result = await _controller.Update(userId, command);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<UserDto>(actionResult.Value);
            Assert.Equal(updatedUserDto.Email, returnValue.Email);
            Assert.Equal(updatedUserDto.FirstName, returnValue.FirstName);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenUserIdMismatch()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new UpdateUserCommand(Guid.NewGuid(), "John", "Doe", "john.doe@example.com");

            // Act
            var result = await _controller.Update(userId, command);

            // Assert
            var actionResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Mismatched user ID", actionResult.Value);
        }

        [Fact]
        public async Task Update_Returns500_WhenMediatorThrows()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new UpdateUserCommand(userId, "John", "Doe", "john.doe@example.com");

            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new Exception("Mediator failed"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.Update(userId, command));
        }

    }
}

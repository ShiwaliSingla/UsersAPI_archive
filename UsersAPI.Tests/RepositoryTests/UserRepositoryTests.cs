using Microsoft.EntityFrameworkCore;
using Moq;
using UsersAPI.Infrastructure.DBInteraction;
using UsersAPI.Infrastructure.Repositories;
using UsersAPI.Domain.Entities;
using UsersAPI.Domain.Exceptions;

namespace UnitTests.RepositoryTests
{
    public class UserRepositoryTests
    {
        [Fact]
        public async Task CreateAsync_AddsUserToDatabase()
        {
            var options = new DbContextOptionsBuilder<UserDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new UserDbContext(options);
            var repo = new UserRepository(context);

            var user = new User { Id = Guid.NewGuid(), FirstName = "Test", LastName = "User", Email = "test@example.com" };

            await repo.CreateAsync(user);

            var savedUser = await context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);

            Assert.NotNull(savedUser);
            Assert.Equal("test@example.com", savedUser.Email);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsUser_WhenFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com"
            };

            var options = new DbContextOptionsBuilder<UserDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new UserDbContext(options);
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var repo = new UserRepository(context);

            // Act
            var result = await repo.GetByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            Assert.Equal(user.Email, result.Email);
        }
        [Fact]
        public async Task UpdateAsync_UpdatesUserInDatabase()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var originalUser = new User
            {
                Id = userId,
                FirstName = "Original",
                LastName = "User",
                Email = "original@example.com"
            };

            var options = new DbContextOptionsBuilder<UserDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new UserDbContext(options))
            {
                context.Users.Add(originalUser);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new UserDbContext(options))
            {
                var repo = new UserRepository(context);

                // Update user
                var updatedUser = new User
                {
                    Id = userId,
                    FirstName = "Updated",
                    LastName = "User",
                    Email = "updated@example.com"
                };

                await repo.UpdateAsync(updatedUser);
            }

            // Assert
            using (var context = new UserDbContext(options))
            {
                var userFromDb = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);

                Assert.NotNull(userFromDb);
                Assert.Equal("Updated", userFromDb!.FirstName);
                Assert.Equal("updated@example.com", userFromDb.Email);
            }
        }

        [Fact]
        public async Task GetByIdAsync_ThrowsNotFoundException_WhenUserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var options = new DbContextOptionsBuilder<UserDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new UserDbContext(options);
            var repo = new UserRepository(context);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => repo.GetByIdAsync(userId));
        }
        [Fact]
        public async Task UpdateAsync_ThrowsNotFoundException_WhenUserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var updatedUser = new User
            {
                Id = userId,
                FirstName = "Updated",
                LastName = "User",
                Email = "updated@example.com"
            };

            var options = new DbContextOptionsBuilder<UserDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new UserDbContext(options);
            var repo = new UserRepository(context);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => repo.UpdateAsync(updatedUser));
        }
    }
}

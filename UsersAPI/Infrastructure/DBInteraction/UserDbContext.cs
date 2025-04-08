using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using UsersAPI.Domain.Entities;

namespace UsersAPI.Infrastructure.DBInteraction
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
    }
}

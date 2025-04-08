using UsersAPI.Domain.Interfaces;
using UsersAPI.Infrastructure.DBInteraction;
using UsersAPI.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace UsersAPI.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<UserDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped<IUserRepository, UserRepository>();
            return services;
        }
    }
}

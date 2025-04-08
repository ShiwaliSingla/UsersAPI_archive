using UsersAPI.Middlewares;
using UsersAPI.Infrastructure;
using MediatR;
using System.Reflection;
using UsersAPI.Middlewares.UsersAPI.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddInfrastructure(builder.Configuration.GetConnectionString("DefaultConnection")!);
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Add the middleware before other configurations (like MapControllers)
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Now map controllers
app.MapControllers();

// Finally, run the application
app.Run();

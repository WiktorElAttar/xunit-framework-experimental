using Microsoft.EntityFrameworkCore;
using SampleApp.Domain;
using SampleApp.Infrastructure;
using SampleApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();

    if (!db.Users.Any())
    {
        db.Users.AddRange(
            new User { Name = "Alice Smith", Email = "alice@example.com" },
            new User { Name = "Bob Jones", Email = "bob@example.com" },
            new User { Name = "Charlie Day", Email = "charlie@example.com" }
        );
        db.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/users", async (IUserService userService) =>
    await userService.GetAllUsersAsync());

app.MapPost("/users", async (IUserService userService, string name, string email) =>
    await userService.CreateUserAsync(name, email));

app.MapPut("/users/{id}", async (IUserService userService, int id, string name, string email) =>
    await userService.UpdateUserAsync(id, name, email));

app.Run();

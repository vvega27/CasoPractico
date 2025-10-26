using Microsoft.EntityFrameworkCore;
using CasoPractico.MinimalAPI.Context;
using Task = CasoPractico.MinimalAPI.Model.Task;


var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Register your DbContext with the connection string
builder.Services.AddDbContext<TaskDbContext>(options =>
    options.UseSqlServer(connectionString));
var app = builder.Build();



app.MapGet("/api/tasks", async (TaskDbContext db) =>
    await db.Task.ToListAsync());

app.Run();

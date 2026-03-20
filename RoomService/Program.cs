using Microsoft.EntityFrameworkCore;
using RoomService.Data;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<RoomDbContext>(options =>
    options.UseSqlite("Data Source=roomsv3.db")); 


builder.Services.AddScoped<RoomService.Filters.ApiKeyFilter>();
builder.Services.AddControllers(options =>
{
    options.Filters.AddService<RoomService.Filters.ApiKeyFilter>();
});

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RoomDbContext>();
    db.Database.EnsureCreated();
}
app.MapOpenApi();
app.MapScalarApiReference();

if (app.Environment.IsDevelopment())
{
    // lämna tomt eller ta bort if-blocket helt
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
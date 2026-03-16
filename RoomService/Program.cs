using Microsoft.EntityFrameworkCore;
using RoomService.Data;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<RoomDbContext>(options =>
    options.UseSqlite("Data Source=rooms.db"));


builder.Services.AddScoped<RoomService.Filters.ApiKeyFilter>();
builder.Services.AddControllers(options =>
{
    options.Filters.AddService<RoomService.Filters.ApiKeyFilter>();
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
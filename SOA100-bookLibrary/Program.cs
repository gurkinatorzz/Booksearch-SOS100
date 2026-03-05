using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using SOA100_bookLibrary.Data;
using SOA100_bookLibrary.Security;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//Skapa en cors-policy och tillåt react-app på annan domän
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactAppPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();        
    });

});

/* --------------------------------------------------------------------------------------- */
//konfigurera DbContext för SQLite
builder.Services.AddDbContext<BookLibraryDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});
/* --------------------------------------------------------------------------------------- */


var app = builder.Build();

//applicera databasmigration vid startup
try
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var dbContext = services.GetRequiredService<BookLibraryDbContext>();
        dbContext.Database.Migrate();
        DbSeeder.Seed(dbContext); //Fyller på databasen med information vid startup (om den är tom)
    }
}
catch (Exception ex)
{
    app.Logger.LogError(ex, "Migration failed at startup");
    throw;
}


// Configure the HTTP request pipeline.
/*if (app.Environment.IsDevelopment())*/
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<ApiKeyMiddleware>();

app.MapControllers();

app.UseCors("ReactAppPolicy");

app.Run();
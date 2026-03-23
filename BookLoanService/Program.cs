using BookLoanService.Data;
using BookLoanService.Services;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//Konfiguera DbContext för sqlite
builder.Services.AddDbContext<BookLoanServiceDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

//lägg till http till till service
builder.Services.AddHttpClient<LoanService>((serviceProvider, httpClient) =>
{
    // Hämta config
    var config =  serviceProvider.GetRequiredService<IConfiguration>();
    
    //Hämta adress till loanservice
    string adress = config.GetValue<string>("BookLoanService") ?? "";
    
    httpClient.BaseAddress = new Uri(adress);
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactAppPolicy", policy =>
    { 
        policy.WithOrigins("http://localhost:5174")
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

var app = builder.Build(); 

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext  = services.GetRequiredService<BookLoanServiceDbContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
// if ( app.Environment.IsDevelopment())


{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseCors("ReactAppPolicy");

app.Run();
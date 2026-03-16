using BookLoanService.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//lägg till http till till service
builder.Services.AddHttpClient<LoanService>((serviceProvider, httpClient) =>
{
    httpClient.BaseAddress = new Uri("http://localhost:5258/BookLoan");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// if ( app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
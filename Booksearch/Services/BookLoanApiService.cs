using System.Net.Http;
using System.Net.Http.Json;
using Booksearch.Models;

namespace Booksearch.Services;

public class BookLoanApiService
{
    private readonly HttpClient _httpClient;

    public BookLoanApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task CreateLoan(BookLoan model)
    {
        await _httpClient.PostAsJsonAsync("api/BookLoan", model);
    }
    
    public async Task<List<BookLoan>> GetActiveLoans()
    {
        return await _httpClient.GetFromJsonAsync<List<BookLoan>>("api/BookLoan/active") 
               ?? new List<BookLoan>();
    }
    public async Task ReturnLoan(int id)
    {
        var response = await _httpClient.PutAsync($"api/BookLoan/return/{id}", null);
        Console.WriteLine($"Return statuskod: {response.StatusCode}");
    }
    public async Task UpdateLoan(BookLoan model)
    {
        await _httpClient.PutAsJsonAsync($"api/BookLoan/{model.Id}", model);
    }
}
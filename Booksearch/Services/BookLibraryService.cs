using Booksearch.Models.BookLibraryDtos;
using System.Net.Http.Json;
namespace Booksearch.Services;

public class BookLibraryService
{
    private readonly HttpClient _httpClient;

    public BookLibraryService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    //funktion för att hämta böcker med detaljer (allt som finns i BookListDto-filen)
    public async Task<List<BookListDto>> GetBooksWithDetail()
    {
        var fullUrl = new Uri(_httpClient.BaseAddress!, "book/with-details");
        
        var response = await _httpClient.GetAsync("book/with-details");
        var body = await response.Content.ReadAsStringAsync();
        
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"API call failed\nURL: {fullUrl}\nStatus: {(int)response.StatusCode} {response.ReasonPhrase}\nBody:\n{body}"
            );
        }

        var data = await response.Content.ReadFromJsonAsync<List<BookListDto>>();
        return data ?? new List<BookListDto>();
    }
    
    //Skapa bok
    public async Task CreateBook(string title, int year, int authorId, int categoryId)
    {
        var dto = new BookCreateDto()
        {
            Title = title.Trim(),
            Year = year,
            AuthorId = authorId,
            CategoryId = categoryId
        };
        
        var fullUrl = new Uri(_httpClient.BaseAddress!, "book");
        
        var response = await _httpClient.PostAsJsonAsync("book", dto);
        var body = await response.Content.ReadAsStringAsync();
        
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"API call failed\nURL: {fullUrl}\nStatus: {(int)response.StatusCode} {response.ReasonPhrase}\nBody:\n{body}"
                );
        }
    }

    //skapa författare
    public async Task<AuthorListDto> CreateAuthor(string name)
    {
        var dto = new AuthorCreateDto {Name =  name.Trim() };
        
        var fullUrl = new Uri(_httpClient.BaseAddress!, "author");
        var response = await _httpClient.PostAsJsonAsync("author", dto);
        var body = await response.Content.ReadAsStringAsync();
        
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"API call failed\nURL: {fullUrl}\nStatus: {(int)response.StatusCode} {response.ReasonPhrase}\nBody:\n{body}"
            );
        }
        var created = await response.Content.ReadFromJsonAsync<AuthorListDto>();
        return created ?? throw new Exception("API returnerade inga författare");
    }
    
    //skapa Kategori
    public async Task<CategoryListDto> CreateCategory(string name)
    {
        var dto = new CategoryCreateDto() {Name =  name.Trim() };
        
        var fullUrl = new Uri(_httpClient.BaseAddress!, "category");
        var response = await _httpClient.PostAsJsonAsync("category", dto);
        var body = await response.Content.ReadAsStringAsync();
        
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"API call failed\nURL: {fullUrl}\nStatus: {(int)response.StatusCode} {response.ReasonPhrase}\nBody:\n{body}"
            );
        }
        var created = await response.Content.ReadFromJsonAsync<CategoryListDto>();
        return created ?? throw new Exception("API returnerade inga kategorier");
    }
    
    //Hämta in författare
    public async Task<List<AuthorListDto>> GetAuthors()
    {
        var fullUrl = new Uri(_httpClient.BaseAddress!, "author");

        var response = await _httpClient.GetAsync("author");
        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"API call failed\nURL: {fullUrl}\nStatus: {(int)response.StatusCode} {response.ReasonPhrase}\nBody:\n{body}"
            );
        }

        var data = await response.Content.ReadFromJsonAsync<List<AuthorListDto>>();
        return data ?? new List<AuthorListDto>();
    }
    
    //Hämta in Kategorier
    public async Task<List<CategoryListDto>> GetCategories()
    {
        var fullUrl = new Uri(_httpClient.BaseAddress!, "category");

        var response = await _httpClient.GetAsync("category");
        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"API call failed\nURL: {fullUrl}\nStatus: {(int)response.StatusCode} {response.ReasonPhrase}\nBody:\n{body}"
            );
        }

        var data = await response.Content.ReadFromJsonAsync<List<CategoryListDto>>();
        return data ?? new List<CategoryListDto>();
    }
    //Ta bort bok
    public async Task DeleteBook(int id)
    {
        var fullUrl = new Uri(_httpClient.BaseAddress!, $"book/{id}");

        var response = await _httpClient.DeleteAsync($"book/{id}");
        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"API call failed\nURL: {fullUrl}\nStatus: {(int)response.StatusCode} {response.ReasonPhrase}\nBody:\n{body}"
            );
        }
    }

    //hämta in information för att sedan kunna uppdatera
    public async Task<BookDetailsDto> GetBookById(int id)
    {
        var fullUrl = new Uri(_httpClient.BaseAddress!, $"book/{id}");
        var response = await _httpClient.GetAsync($"book/{id}");
        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception(
                $"API call failed\nURL: {fullUrl}\nStatus: {(int)response.StatusCode} {response.ReasonPhrase}\nBody:\n{body}");
        
        var data = await response.Content.ReadFromJsonAsync<BookDetailsDto>();
        return data ?? throw new Exception("API returnerade ingen bok");
    }

    //uppdatera bok
    public async Task UpdateBook(int id, string title, int year, int authorId, int categoryId)
    {
        var dto = new BookUpdateDto()
        {
            Title = title.Trim(),
            Year = year,
            AuthorId = authorId,
            CategoryId = categoryId
        };
        
        var fullUrl = new Uri(_httpClient.BaseAddress!, $"book/{id}");
        var response = await _httpClient.PutAsJsonAsync($"book/{id}", dto);
        var body = await response.Content.ReadAsStringAsync();
        
        if (!response.IsSuccessStatusCode) throw new Exception
                ($"API call failed\nURL: {fullUrl}\nStatus: {(int)response.StatusCode} {response.ReasonPhrase}\nBody:\n{body}");
    }
}
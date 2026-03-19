using System.Text;
using System.Text.Json;

namespace Booksearch.Services
{
    public class UserApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _apiKey;

        public UserApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            // ✅ Remove API key handling - it's now handled in Program.cs like BookLibraryService
        }

        private void SetApiKeyHeader()
        {
            // ✅ Remove this method or make it empty - API key is now set in Program.cs
            // The HttpClient already has the API key as default header
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            try
            {
                
                var response = await _httpClient.GetAsync("/api/users");
            
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<UserDto>>(json, new JsonSerializerOptions 
                    { 
                        PropertyNameCaseInsensitive = true 
                    }) ?? new List<UserDto>();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"API call failed: {response.StatusCode} - {errorContent}");
                }
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Connection error to UserService: {ex.Message}");
            }
        }

        public async Task<UserDto?> GetUserAsync(int id)
        {
            try
            {
                
                var response = await _httpClient.GetAsync($"/api/users/{id}");
            
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<UserDto>(json, new JsonSerializerOptions 
                    { 
                        PropertyNameCaseInsensitive = true 
                    });
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"API call failed: {response.StatusCode} - {errorContent}");
                }
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Connection error: {ex.Message}");
            }
        }

        public async Task<bool> CreateUserAsync(UserDto user)
        {
            try
            {
                
                var json = JsonSerializer.Serialize(user);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
            
                var response = await _httpClient.PostAsync("/api/users", content);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Create user failed: {response.StatusCode} - {errorContent}");
                }
                
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Connection error: {ex.Message}");
            }
        }

        public async Task<bool> UpdateUserAsync(UserDto user)
        {
            try
            {
                
                var json = JsonSerializer.Serialize(user);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
            
                var response = await _httpClient.PutAsync($"/api/users/{user.Id}", content);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Update user failed: {response.StatusCode} - {errorContent}");
                }
                
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Connection error: {ex.Message}");
            }
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            try
            {
                
                var response = await _httpClient.DeleteAsync($"/api/users/{id}");
                
                if (!response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Delete user failed: {response.StatusCode} - {errorContent}");
                }
                
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Connection error: {ex.Message}");
            }
        }

        public async Task<bool> SetUserPasswordAsync(int userId, string password)
        {
            try
            {
                var requestData = new { Password = password };
                var json = JsonSerializer.Serialize(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"/api/password/{userId}/set", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Set password failed: {response.StatusCode} - {errorContent}");
                }

                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Connection error: {ex.Message}");
            }
        }
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public bool IsAdmin { get; set; }
        public string? PasswordHash { get; set; }
    }
}

using System.Text;
using System.Text.Json;

namespace Booksearch.Services
{
    public class UserApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public UserApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        // Login method for authentication
        public async Task<LoginResult?> LoginAsync(string email, string password)
        {
            try
            {
                var loginRequest = new
                {
                    Email = email,
                    Password = password
                };

                var json = JsonSerializer.Serialize(loginRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/users/login", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<LoginResult>(responseJson, new JsonSerializerOptions 
                    { 
                        PropertyNameCaseInsensitive = true 
                    });
                }
                
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // API validation method
        public async Task<bool> ValidateApiKeyAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/users/validate");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
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

    public class LoginResult
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}

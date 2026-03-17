namespace BookLoanService.Services;

public class LoanService
{
    private HttpClient _httpClient;

    public LoanService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<WeatherForecast[]> GetWeatherForecast()
    {
        var result = await _httpClient.GetFromJsonAsync<WeatherForecast[]>("WeatherForecast");
        if (result == null)
        {
            return [];
        }

        return result;
    }
}    
    
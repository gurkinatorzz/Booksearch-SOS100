using Booksearch.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

/*------------------GUSTAVS RÖR EJ!!!---------------------------------------------*/
//Lägg till HttpClient till BookLibrary API (Gustavs):
builder.Services.AddHttpClient<BookLibraryService>((serviceProvider, httpClient) =>
{
    // Hämta config
    var config = serviceProvider.GetRequiredService<IConfiguration>();
    
    // Hämta adress till BookLibrary ifrån config
    string adress = config.GetValue<string>("BookLibraryAdress") ?? "";
    
    httpClient.BaseAddress = new Uri(adress);
    httpClient.DefaultRequestHeaders.Add("X-LIBRARY-API-KEY", config["LibraryApiKey"]);
    
});
/*--------------------------------------------------------------------------------*/

// Add HttpClient for UserService API
builder.Services.AddHttpClient<UserApiService>((serviceProvider, httpClient) =>
{
    var config = serviceProvider.GetRequiredService<IConfiguration>();
    
    // Get UserService address from config
    string baseUrl = config.GetValue<string>("UserService:BaseUrl") ?? "https://localhost:7001";
    httpClient.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddSingleton<Booksearch.Services.ReservationService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
    });

builder.Services.AddSingleton<Booksearch.Services.ReservationService>();


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// ↓ LÄGG TILL DENNA RAD (måste vara FÖRE UseAuthorization)
app.UseAuthentication();
// ↑ SLUT

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();

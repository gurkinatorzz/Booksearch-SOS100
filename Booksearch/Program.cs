using Booksearch.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
//Rayan--- Rör Ej!-------------------------------------------------------------

//add httpclient to reservation
builder.Services.AddHttpClient<ReservationService>((serviceProvider, httpClient) =>
{
    //hämta config
    var config = serviceProvider.GetRequiredService<IConfiguration>();
    
    //Hämta adress 
    string adress =config.GetValue<string>("ReservationServiceAdress") ?? "";
   
    httpClient.BaseAddress = new Uri(adress);
});

//WIlliam --- RÖR EJ
//Lägg till httpclient till BookLoan
builder.Services.AddHttpClient<BookLoanApiService>((serviceProvider, httpClient) =>
{
    var config = serviceProvider.GetRequiredService<IConfiguration>();
    string address = config.GetValue<string>("BookLoanService") ?? "";

    httpClient.BaseAddress = new Uri(address);
});
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

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
    });

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

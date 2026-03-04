using Booksearch.Services;

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
builder.Services.AddSingleton<Booksearch.Services.ReservationService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();


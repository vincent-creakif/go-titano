using Blazored.LocalStorage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddOptions();

builder.Services.AddTransient<LocalStorageService>();
builder.Services.AddTransient<BscScanService>();
builder.Services.AddTransient<CoinGeckoService>();
builder.Services.AddScoped<TimeZoneService>();
builder.Services.AddTransient<TitanoService>();

builder.Services.AddBlazoredLocalStorage();

var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environmentName}.json", optional: false, reloadOnChange: true);

// Register service settings
builder.Services.Configure<BscScanSettings>(builder.Configuration.GetSection("BscScanSettings"));
builder.Services.Configure<CoinGeckoSettings>(builder.Configuration.GetSection("CoinGeckoSettings"));

var app = builder.Build();

app.Use(async (context, next) =>
{
    var culture = new CultureInfo("en-US");
    CultureInfo.CurrentCulture = culture;

    await next();
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.UseHttpsRedirection();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

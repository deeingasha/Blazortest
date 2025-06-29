using BlazorApp1.Components;
using BlazorApp1.Components.Core.Auth;
using BlazorApp1.Components.Core.Config;
using BlazorApp1.Components.Core.Http;
using BlazorApp1.Components.Core.Logging;
using BlazorApp1.Components.Core.Mapping;
using BlazorApp1.Components.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using System.Net.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(options =>
    {
        options.DetailedErrors = builder.Environment.IsDevelopment();
    });


// Add logging -- Server side only
builder.Services.AddLogging(logging =>
{
    // logging.AddConsole();
    // logging.AddDebug();
    logging.ClearProviders(); // Clear existing providers
    builder.Services.Configure<SimpleConsoleFormatterOptions>(options =>
     {
         options.TimestampFormat = "[HH:mm:ss] ";
         //  options.IncludeScopes = true;
     });
    logging.AddConsole(options =>
    {
        options.FormatterName = "simple";
    });

    if (builder.Environment.IsDevelopment())
    {
        logging.SetMinimumLevel(LogLevel.Debug); // Show debug logs in development
    }
});

builder.Services.Configure<ApiConfig>(
    builder.Configuration.GetSection("ApiConfig"));

builder.Services.AddScoped<ProtectedSessionStorage>(); // Add ProtectedSessionStorage before http client reg

// HTTP Client configuration
builder.Services.AddScoped<ApiLoggingHandler>();
builder.Services.AddTransient<JwtAuthHandler>(); // Changed to Transient
builder.Services.AddHttpClient<IApiHttpClient, ApiHttpClient>((sp, client) =>
{
    var config = sp.GetRequiredService<IOptions<ApiConfig>>().Value;
    client.BaseAddress = new Uri(config.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(config.Timeout);
})
.AddHttpMessageHandler<ApiLoggingHandler>()
.AddHttpMessageHandler<JwtAuthHandler>(); // Add JWT handler


// Authentication & Authorization
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your-secret-key")), // Get from config
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });


builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

builder.Services.AddScoped<IHospitalService, HospitalService>();
builder.Services.AddScoped<IHospitalMapper, HospitalMapper>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IDepartmentMapper, DepartmentMapper>();
builder.Services.AddScoped<IBankService, BankService>();
builder.Services.AddScoped<IBankMapper, BankMapper>();
builder.Services.AddScoped<IReagentService, ReagentService>();

builder.Services.AddScoped<IDrugService, DrugService>();
builder.Services.AddScoped<IDrugMapper, DrugMapper>();
builder.Services.AddScoped<ILpoService, LpoService>();
builder.Services.AddScoped<IDrugReceiptService, DrugReceiptService>();



builder.Services.AddMemoryCache(); // Added memory cache service



var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

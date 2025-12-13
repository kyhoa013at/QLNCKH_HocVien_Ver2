using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using QLNCKH_HocVien.Client.Pages;
using QLNCKH_HocVien.Client.Services;
using QLNCKH_HocVien.Components;
using QLNCKH_HocVien.Data;
using QLNCKH_HocVien.Middleware;
using QLNCKH_HocVien.Repositories;
using QLNCKH_HocVien.Services;

var builder = WebApplication.CreateBuilder(args);

// ========== LOGGING ==========
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Set minimum log level
builder.Logging.SetMinimumLevel(
    builder.Environment.IsDevelopment() ? LogLevel.Debug : LogLevel.Information);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// ========== AUTHENTICATION ==========
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "QLNCKH_Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.LoginPath = "/login";
        options.LogoutPath = "/api/auth/logout";
        options.AccessDeniedPath = "/access-denied";
        
        // Trả về 401 thay vì redirect cho API calls
        options.Events.OnRedirectToLogin = context =>
        {
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            }
            context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        };
    });

builder.Services.AddAuthorization();

// ========== MEMORY CACHE ==========
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ICacheService, MemoryCacheService>();

// ========== HTTP CONTEXT ==========
builder.Services.AddHttpContextAccessor();

// Cấu hình HttpClient với cookie forwarding
builder.Services.AddScoped(sp =>
{
    var navigationManager = sp.GetRequiredService<NavigationManager>();
    var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
    
    var handler = new HttpClientHandler();
    handler.UseCookies = true;
    
    var client = new HttpClient(handler)
    {
        BaseAddress = new Uri(navigationManager.BaseUri)
    };
    
    // Forward auth cookie từ browser request sang HttpClient
    var httpContext = httpContextAccessor.HttpContext;
    if (httpContext != null)
    {
        var cookies = httpContext.Request.Cookies;
        if (cookies.TryGetValue("QLNCKH_Auth", out var authCookie))
        {
            client.DefaultRequestHeaders.Add("Cookie", $"QLNCKH_Auth={authCookie}");
        }
    }
    
    return client;
});

// ========== DATABASE ==========
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException(
        "Connection string 'DefaultConnection' not found. " +
        "Please set it using: dotnet user-secrets set \"ConnectionStrings:DefaultConnection\" \"<your-connection-string>\"");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// ========== REPOSITORIES (Phase 3) ==========
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ISinhVienRepository, SinhVienRepository>();
builder.Services.AddScoped<IGiaoVienRepository, GiaoVienRepository>();
builder.Services.AddScoped<IChuyenDeRepository, ChuyenDeRepository>();
builder.Services.AddScoped<INopSanPhamRepository, NopSanPhamRepository>();
builder.Services.AddScoped<IHoiDongRepository, HoiDongRepository>();
builder.Services.AddScoped<IKetQuaRepository, KetQuaRepository>();
builder.Services.AddScoped<IXepGiaiRepository, XepGiaiRepository>();
builder.Services.AddScoped<INguoiDungRepository, NguoiDungRepository>();

// ========== BUSINESS SERVICES (Phase 3) ==========
builder.Services.AddScoped<ISinhVienService, SinhVienBusinessService>();
builder.Services.AddScoped<IGiaoVienService, GiaoVienBusinessService>();
builder.Services.AddScoped<IChuyenDeService, ChuyenDeBusinessService>();

// ========== CLIENT SERVICES (cho Blazor Server-side rendering) ==========
builder.Services.AddScoped<SinhVienService>();
builder.Services.AddScoped<GiaoVienService>();
builder.Services.AddScoped<ChuyenDeNCKHService>();
builder.Services.AddScoped<NopSanPhamService>();
builder.Services.AddScoped<HoiDongService>();
builder.Services.AddScoped<KetQuaService>();
builder.Services.AddScoped<XepGiaiService>();
builder.Services.AddScoped<AuthService>();

// ========== CONTROLLERS & MUD BLAZOR ==========
builder.Services.AddControllers();
builder.Services.AddMudServices();

var app = builder.Build();

// ========== MIDDLEWARE PIPELINE ==========

// Global Exception Handler (Phase 3)
app.UseGlobalExceptionHandler();

// Request Logging (Phase 3)
app.UseRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStaticFiles();

// ========== AUTHENTICATION MIDDLEWARE ==========
app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(QLNCKH_HocVien.Client._Imports).Assembly);

// Map API Controllers
app.MapControllers();

// ========== STARTUP LOGGING ==========
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("========================================");
logger.LogInformation("QLNCKH_HocVien Application Starting...");
logger.LogInformation("Environment: {Environment}", app.Environment.EnvironmentName);
logger.LogInformation("========================================");

app.Run();

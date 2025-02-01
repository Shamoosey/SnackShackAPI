using AutoMapper;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SnackShackAPI;
using SnackShackAPI.Controllers;
using SnackShackAPI.Database;
using SnackShackAPI.Services;
using SnackShackAPI.SignalR;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IAccountService, AccountService>();
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<IExchangeRateService, ExchangeRateService>();
builder.Services.AddSignalR();

// Add services to the container.
var configuration = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddSingleton(p => new MapperConfiguration(cfg => {
    cfg.AddProfile(new MappingProfile());
}).CreateMapper());


builder.Services.AddDbContext<SnackShackContext>(options => {
    options.UseLazyLoadingProxies();
    options.UseSqlServer(configuration["ConnectionStrings:DefaultConnection"]);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .WithOrigins("http://localhost:4200") // Allow only your Angular app
            .AllowAnyHeader()                     // Allow any headers (e.g., Authorization)
            .AllowAnyMethod()                     // Allow GET, POST, PUT, DELETE, etc.
            .AllowCredentials());                 // Allow cookies or credentials
});

builder.Services
.AddAuthentication(options => {
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddDiscord(options =>
{
    options.ClientId = configuration["Discord:ClientId"];
    options.ClientSecret = configuration["Discord:ClientSecret"];
    //options.CallbackPath = configuration["Discord:RedirectUri"];
    //options.TokenEndpoint = "https://discord.com/api/oauth2/token";

}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]))
    };
});

builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseDeveloperExceptionPage();
    app.UseSwaggerUI(x => x.SwaggerEndpoint("/swagger/v1/swagger.json", "SnackShackAPI v1"));
}

app.UseRouting();

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();


var idProvider = new UserIdProvider();
GlobalHost.DependencyResolver.Register(typeof(Microsoft.AspNet.SignalR.IUserIdProvider), () => idProvider);


app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<NotificationHub>("/notifications");
});

//app.MapControllers();

string port = configuration["Application:Port"];

app.Run($"https://localhost:{port}");

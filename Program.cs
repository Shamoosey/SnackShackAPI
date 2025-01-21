using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SnackShackAPI;
using SnackShackAPI.Controllers;
using SnackShackAPI.Database;
using SnackShackAPI.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IAccountService, AccountService>();
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<IExchangeRateService, ExchangeRateService>();

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

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    // Set cookie options
    options.Cookie.SameSite = SameSiteMode.None;  // Allow cross-origin cookies
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;  // Always use secure cookies (HTTPS)
})
.AddJwtBearer(options =>
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

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

string port = configuration["Application:Port"];

app.Run($"https://localhost:{port}");

using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SnackShackAPI;
using SnackShackAPI.Database;
using SnackShackAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IAccountService, AccountService>();
builder.Services.AddTransient<IExchangeRateService, ExchangeRateService>();

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
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.Authority = configuration["Authentication:Domain"];
    options.Audience = configuration["Authentication:Audience"];
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = true,
        ValidateIssuerSigningKey = true
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

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseRouting(); 

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

string port = configuration["Application:Port"];

app.Run($"https://localhost:{port}");

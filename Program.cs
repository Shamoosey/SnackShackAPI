using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SnackShackAPI;
using SnackShackAPI.Database;
using SnackShackAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IUserService, UserService>();

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

app.UseAuthorization();

app.MapControllers();

string port = configuration["Application:Port"];

app.Run($"https://localhost:{port}");

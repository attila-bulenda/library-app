using library_app.Contracts;
using library_app.Core.Configurations;
using library_app.Identity;
using library_app.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using library_app.Data.Context;
using library_app.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("LibraryAppDbConnectionString");
builder.Services.AddDbContext<LibraryAppDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddIdentityCore<LibraryUser>()
    .AddRoles<IdentityRole>()
    .AddTokenProvider<DataProtectorTokenProvider<LibraryUser>>("LibraryAppApi")
    .AddEntityFrameworkStores<LibraryAppDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddAutoMapper(typeof(AutoMapperConfig));
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IBooksRepository, BooksRepository>();
builder.Services.AddScoped<IMembersRepository, MembersRepository>();
builder.Services.AddScoped<IAuthManager, AuthManager>();
builder.Services.AddScoped<MembersService>();
builder.Services.AddScoped<BooksService>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]))
    };
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});
builder.WebHost.UseUrls("http://+:80");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
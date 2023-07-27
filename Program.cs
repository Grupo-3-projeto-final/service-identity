using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Identity.Domain.Entities;
using Identity.Infrastructure.Repositories;
using Identity.Infrastructure.Repositories.Interfaces;
using Identity.Domain.Services;
using Identity.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthorization();

builder.Services.AddDbContext<AppContext>(options =>
    options.UseMySql(Environment.GetEnvironmentVariable("IDENTITY_CONNECTION_DB"), new MySqlServerVersion(new Version(8, 0, 21))));

builder.Services.AddScoped<ICrypto, Crypto>();
builder.Services.AddScoped<ITokenJwt, TokenJwt>();
builder.Services.AddScoped<IRepository<User>, Repository<User>>();
builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAnyOrigin", builder =>
            {
            builder.AllowAnyOrigin()
                   .AllowAnyHeader()
                   .AllowAnyMethod();
            });

        });
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();

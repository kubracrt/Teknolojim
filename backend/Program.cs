using Microsoft.EntityFrameworkCore;
using Context;
using StackExchange.Redis;
using Microsoft.Extensions.DependencyInjection;
using Entities;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost:6379")); // redis adresiniz

builder.Services.AddScoped<IDatabase>(sp =>
{
    var multiplexer = sp.GetRequiredService<IConnectionMultiplexer>();
    return multiplexer.GetDatabase();
});

// PostgreSQL bağlantı dizesini al ve eCommerceContext'i yapılandır
builder.Services.AddDbContext<eCommerceContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// CORS politikasını ekle
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
        builder.AllowAnyOrigin() // Bütün origin'lere izin ver
               .AllowAnyMethod() // Bütün HTTP metotlarına izin ver
               .AllowAnyHeader()); // Bütün header'lara izin ver
});

// Swagger/OpenAPI desteği ekle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// API Controller'larını ekle
builder.Services.AddControllers();

// Diğer servisleri ekle
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<ProductService>();
// builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<RolesService>();
builder.Services.AddScoped<RedisProductService>();
builder.Services.AddScoped<ShoppingCardService>();
builder.Services.AddScoped<KafkaProducerService>();
builder.Services.AddScoped<ViewEventService>();
builder.Services.AddHostedService<OrderConsumerService>();

var app = builder.Build();

// Geliştirme ortamında hata ayıklamayı etkinleştir
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger(); // Swagger'ı etkinleştir
    app.UseSwaggerUI(); // Swagger UI'yi yapılandır
}

using (var scope = app.Services.CreateScope())
{
    var kafkaService = scope.ServiceProvider.GetRequiredService<KafkaProducerService>();
}



// CORS middleware'ini ekle
app.UseCors("AllowAllOrigins");

// API Controller'larını yönlendir
app.MapControllers();

// Uygulama çalıştır
app.Run();


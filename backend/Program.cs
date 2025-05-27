using Context;
using Microsoft.EntityFrameworkCore;
using Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Redis bağlantısı
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost:6379"));

builder.Services.AddScoped<IDatabase>(sp =>
{
    var multiplexer = sp.GetRequiredService<IConnectionMultiplexer>();
    return multiplexer.GetDatabase();
});

// DbContext ayarları
builder.Services.AddDbContext<eCommerceContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Diğer servisler
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<RolesService>();
builder.Services.AddScoped<RedisProductService>();
builder.Services.AddScoped<ShoppingCardService>();
builder.Services.AddScoped<KafkaProducerService>();


// Hosted service (arka plan servisi)
builder.Services.AddHostedService<OrderConsumerService>();
builder.Services.AddHostedService<OrderViewConsumerService>();

// SignalR
builder.Services.AddSignalR();

// Swagger ve API Explorer
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS politikası
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder.WithOrigins("http://localhost:4200")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

// Controller ekle
builder.Services.AddControllers();

// Build uygulama
var app = builder.Build();

// Middleware kullanımları
app.UseCors("CorsPolicy");

//Hub'ler
app.MapHub<OrderHub>("/orderhub");
app.MapHub<ProductViewHub>("/productviewhub");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

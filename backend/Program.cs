using Context;
using Microsoft.EntityFrameworkCore;
using Prometheus;
using Services;
using StackExchange.Redis;
using Prometheus;
using System.Diagnostics;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.PostgreSQL;
using Serilog.Context;

var builder = WebApplication.CreateBuilder(args);


var cpuCollector = new CpuMetricsCollector();

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

// Loglama ayarları
Logger log = new LoggerConfiguration()
  .WriteTo.Console()
  .WriteTo.File("logs/log.txt")
  .WriteTo.PostgreSQL(builder.Configuration.GetConnectionString("DefaultConnection"), "logs",
     needAutoCreateTable: true,
     columnOptions: new Dictionary<string, ColumnWriterBase>
     {
          {"message", new RenderedMessageColumnWriter()},
          {"message_template", new MessageTemplateColumnWriter()},
          {"level", new LevelColumnWriter()},
          {"time_stamp",new TimestampColumnWriter()},
          {"exception", new ExceptionColumnWriter()},
          {"log_event", new LogEventSerializedColumnWriter()},
     })
     .WriteTo.Seq("http://localhost:5341/#/events?range=1d")
    .Enrich.FromLogContext()
    .MinimumLevel.Information()
  .CreateLogger();


// Serilog'u kullanmak için
builder.Host.UseSerilog(log);


// Build uygulama
var app = builder.Build();

// Serilog ile istek loglama
app.UseSerilogRequestLogging();


//Histrogram oluşturma
var httpRequestDuration = Metrics.CreateHistogram(
    "reques_duraiton_seconds",
    "Apı istek süresi",
    new HistogramConfiguration
    {
        Buckets = Histogram.LinearBuckets(start: 0.01, width: 0.05, count: 20)
    }
);


app.Use(async (context, next) =>
{
    var stopwatch = Stopwatch.StartNew();

    await next();

    stopwatch.Stop();
    httpRequestDuration.Observe(stopwatch.Elapsed.TotalSeconds);

});

// Middleware kullanımları
app.UseCors("CorsPolicy");


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapMetrics();
app.MapControllers();

app.UseMiddleware<backend.Middlewares.RequestLoggingMiddleware>();


app.Run();

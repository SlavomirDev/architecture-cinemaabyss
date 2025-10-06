using CinemaAbyss.Proxy.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

// Register services
builder.Services.AddScoped<IProxyService, ProxyService>();
builder.Services.AddSingleton<IMigrationService, MigrationService>();

var app = builder.Build();

// Configure pipeline
app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();
app.UseAuthorization();

app.MapControllers();

app.Run();
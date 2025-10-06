using CinemaAbyss.Events.Kafka;
using CinemaAbyss.Events.Kafka.Consumers;
using CinemaAbyss.Events.Kafka.Factory;
using CinemaAbyss.Events.Kafka.Producers;

using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var portEnv = Environment.GetEnvironmentVariable("PORT");
var port = string.IsNullOrEmpty(portEnv) ? 5000 : int.Parse(portEnv);
builder.WebHost.UseUrls($"http://*:{port}");

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Json
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.WriteIndented = true;
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
});

// Kafka
builder.Services.Configure<KafkaConfiguration>(config =>
{
    config.Servers = builder.Configuration
        .GetValue<string>("KAFKA_BROKERS") ?? "kafka:9092";
});

builder.Services.Configure<TopicConfiguration>(config =>
{
    config.MovieEventsTopicName = builder.Configuration.GetValue<string>("MOVIE_EVENTS_TOPIC") ?? "movie-events";
    config.PaymentEventsTopicName = builder.Configuration.GetValue<string>("PAYMENT_EVENTS_TOPIC") ?? "payment-events";
    config.UserEventsTopicName = builder.Configuration.GetValue<string>("USER_EVENTS_TOPIC") ?? "user-events";
});

builder.Services.AddSingleton<KafkaFactory>();
builder.Services.AddSingleton<IKafkaConsumerFactory>(sp => sp.GetRequiredService<KafkaFactory>());
builder.Services.AddSingleton<IKafkaProducerFactory>(sp => sp.GetRequiredService<KafkaFactory>());

builder.Services.AddSingleton<MovieEventMessageProducer>();
builder.Services.AddSingleton<PaymentEventMessageProducer>();
builder.Services.AddSingleton<UserEventMessageProducer>();

// Configure Hosted Services
builder.Services.AddHostedService<MovieEventsConsumer>();
builder.Services.AddHostedService<PaymentEventsConsumer>();
builder.Services.AddHostedService<UserEventsConsumer>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

await Task.Delay(5000);
await app.RunAsync();
namespace CinemaAbyss.Proxy.Services;

public class MigrationService : IMigrationService
{
    private ProxyConfig _config;

    public MigrationService(IConfiguration configuration)
    {
        _config = new ProxyConfig
        {
            MonolithUrl = configuration["MONOLITH_URL"] ?? "http://monolith:8080",
            MoviesServiceUrl = configuration["MOVIES_SERVICE_URL"] ?? "http://movies-service:8081",
            EventsServiceUrl = configuration["EVENTS_SERVICE_URL"] ?? "http://events-service:8082",
            MoviesMigrationPercent = int.Parse(configuration["MOVIES_MIGRATION_PERCENT"] ?? "0"),
            GradualMigration = bool.Parse(configuration["GRADUAL_MIGRATION"] ?? "true")
        };
    }

    public bool ShouldRouteToNewService(string serviceName)
    {
        if (!_config.GradualMigration)
            return false;

        return serviceName.ToLower() switch
        {
            "movies" => Random.Shared.Next(100) < _config.MoviesMigrationPercent,
            "events" => true, // Events всегда идет в новый сервис
            _ => false
        };
    }

    public void UpdateMigrationPercent(string serviceName, int percent)
    {
        if (serviceName.ToUpper() == "MOVIES")
        {
            _config.MoviesMigrationPercent = percent;
        }
    }

    public ProxyConfig GetConfig() => _config;
}

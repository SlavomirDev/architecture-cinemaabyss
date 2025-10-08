namespace CinemaAbyss.Proxy.Services;

public class ProxyService : IProxyService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMigrationService _migrationService;
    private readonly ILogger<ProxyService> _logger;

    public  ProxyService(IHttpClientFactory httpClientFactory, IMigrationService migrationService, ILogger<ProxyService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _migrationService = migrationService;
        _logger = logger;
    }

    public async Task<HttpResponseMessage> ForwardRequestAsync(HttpRequest request, string serviceName, string path)
    {
        try
        {
            var targetUrl = GetTargetUrl(serviceName, path);
            _logger.LogInformation($"Forwarding {request.Method} {request.Path} to {targetUrl}");

            var client = _httpClientFactory.CreateClient();
            var proxyRequest = new HttpRequestMessage
            {
                RequestUri = new Uri(targetUrl),
                Method = new HttpMethod(request.Method)
            };

            foreach (var header in request.Headers)
            {
                if (!header.Key.Equals("Host", StringComparison.OrdinalIgnoreCase))
                {
                    proxyRequest.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
                }
            }

            if (request.Method != "GET" && request.Method != "DELETE" && request.Body != null)
            {
                using var streamReader = new StreamReader(request.Body);
                var bodyContent = await streamReader.ReadToEndAsync();
                proxyRequest.Content = new StringContent(bodyContent, System.Text.Encoding.UTF8, request.ContentType ?? "application/json");
            }

            return await client.SendAsync(proxyRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error forwarding request to {ServiceName}", serviceName);
            throw;
        }
    }

    private string GetTargetUrl(string serviceName, string path)
    {
        var config = _migrationService.GetConfig();

        // Для events всегда используем новый сервис
        if (serviceName.Equals("events", StringComparison.OrdinalIgnoreCase))
        {
            return $"{config.EventsServiceUrl}{path}";
        }

        // Для movies проверяем миграцию
        if (serviceName.Equals("movies", StringComparison.OrdinalIgnoreCase))
        {
            var useNewService = _migrationService.ShouldRouteToNewService("MOVIES");
            var baseUrl = useNewService ? config.MoviesServiceUrl : config.MonolithUrl;
            return $"{baseUrl}{path}";
        }

        // Все остальное идет в монолит
        return $"{config.MonolithUrl}{path}";
    }
}

public class ProxyConfig
{
    public string MonolithUrl { get; set; } = "http://monolith:8080";
    public string MoviesServiceUrl { get; set; } = "http://movies-service:8081";
    public string EventsServiceUrl { get; set; } = "http://events-service:8082";
    public int MoviesMigrationPercent { get; set; } = 0;
    public bool GradualMigration { get; set; } = true;
}
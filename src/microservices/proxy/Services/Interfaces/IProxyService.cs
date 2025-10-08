namespace CinemaAbyss.Proxy.Services
{
    public interface IProxyService
    {
        Task<HttpResponseMessage> ForwardRequestAsync(HttpRequest request, string serviceName, string path);
    }
}

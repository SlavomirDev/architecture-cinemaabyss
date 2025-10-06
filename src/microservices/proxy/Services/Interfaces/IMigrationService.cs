namespace CinemaAbyss.Proxy.Services
{
    public interface IMigrationService
    {
        ProxyConfig GetConfig();
        bool ShouldRouteToNewService(string serviceName);
        void UpdateMigrationPercent(string serviceName, int percent);
    }
}

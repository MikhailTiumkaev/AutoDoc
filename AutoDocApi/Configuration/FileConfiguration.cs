using AutoDocApi.FileServices;

namespace AutoDocApi.Configuration;
public static class FileConfiguration
{
    public static void AddFileServices(this IServiceCollection services)
    {
        services.AddScoped<IFileService, FileService>();
        
    }
}
using AutoDocApi.Database;
using Microsoft.EntityFrameworkCore;

public static class MigrateDatabaseExtension
{
    public static void MigrateDatabase(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
        using var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();        
        context!.Database.Migrate();
    }
}
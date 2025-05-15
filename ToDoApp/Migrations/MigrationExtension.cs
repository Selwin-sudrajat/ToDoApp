using Microsoft.EntityFrameworkCore;
using ToDoApp;
using ToDoApp.Models;

namespace ToDoApp.Extensions
{
    
    public static class MigrationExtension
    {
        public static void ApplyMigration(this IApplicationBuilder app)
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();

            using ToDoDbContext context = scope.ServiceProvider.GetRequiredService<ToDoDbContext>();
            context.Database.Migrate();
        }
    }
}

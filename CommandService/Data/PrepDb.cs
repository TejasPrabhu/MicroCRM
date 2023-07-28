using CommandService.Data;
using CommandService.Models;
using CommandService.SyncDataServices.Grpc;
using Microsoft.EntityFrameworkCore;

namespace CommandService.Data
{
    public static class PrepDb
    {
        public static void PrePopulation(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetService<AppDbContext>();

                // Apply database migration
                dbContext.Database.Migrate();

                // Continue with data seeding
                var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>();
                var platforms = grpcClient.ReturnAllPlatforms();
                SeedData(serviceScope.ServiceProvider.GetService<ICommandRepo>(), platforms);
            }
        }

        private static void SeedData(ICommandRepo? commandRepo, IEnumerable<Platform> platforms)
        {
            System.Console.WriteLine("Seeding new Platforms");
            foreach (var plat in platforms)
            {
                if (!commandRepo.ExternalPlatformExists(plat.ExternalId))
                {
                    commandRepo.CreatePlatform(plat);
                }
                commandRepo.SaveChanges();
            }
        }
    }
}

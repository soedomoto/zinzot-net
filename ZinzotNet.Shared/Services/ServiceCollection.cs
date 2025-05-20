using Microsoft.Extensions.DependencyInjection;
using Radzen;

namespace ZinzotNet.Services {
    public static class ServiceCollection {
        public static void Register(IServiceCollection services) {
            services.AddRadzenComponents();

            services.AddSingleton<ISupabaseService, SupabaseService>();
            services.AddSingleton<IS3Service, S3Service>();
            services.AddScoped<CollectionService>();
            services.AddScoped<TableReferenceState>();
            services.AddScoped<DetailReferenceState>();
        }
    }
}
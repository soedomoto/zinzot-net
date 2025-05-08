using Microsoft.Extensions.DependencyInjection;
using Syncfusion.Blazor;

namespace ZinzotNet.Services {
    public static class ServiceCollection {
        public static void Register(IServiceCollection services) {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mzg0NzcwOUAzMjM5MmUzMDJlMzAzYjMyMzkzYkQvTFpOU21iWHZHNWNoU2pKcTZaZFdPMjE2U29raDZnOEZkTTdQbEhhYVk9");

            services.AddSyncfusionBlazor();
            services.AddAntDesign();
            services.AddScoped<ISupabaseService, SupabaseService>();
            services.AddSingleton<IS3Service, S3Service>();
            services.AddScoped<CollectionService>();
            services.AddScoped<TableReferenceState>();
            services.AddScoped<DetailReferenceState>();
        }
    }
}
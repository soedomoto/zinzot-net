using Microsoft.Extensions.Logging;
using ZinzotNet.Shared.Services;
using ZinzotNet.Services;
using Syncfusion.Blazor;

namespace ZinzotNet;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mzg0NzcwOUAzMjM5MmUzMDJlMzAzYjMyMzkzYkQvTFpOU21iWHZHNWNoU2pKcTZaZFdPMjE2U29raDZnOEZkTTdQbEhhYVk9");

		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();
		builder.Services.AddSyncfusionBlazor();

		builder.Services.AddSingleton<IFormFactor, FormFactor>();
		builder.Services.AddSingleton<IFormFactor, FormFactor>();
		builder.Services.AddScoped<ISupabaseService, SupabaseService>();
		builder.Services.AddSingleton<IS3Service, S3Service>();
		builder.Services.AddScoped<TableReferenceState>();
		builder.Services.AddScoped<DetailReferenceState>();
		builder.Services.AddSingleton<SampleService>();
		builder.Services.AddSingleton<SampleService>();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}

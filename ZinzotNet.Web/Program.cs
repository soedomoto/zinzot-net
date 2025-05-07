using ZinzotNet.Web.Components;
using ZinzotNet.Shared.Services;
using ZinzotNet.Web.Services;
using ZinzotNet.Services;
using Syncfusion.Blazor;

Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mzg0NzcwOUAzMjM5MmUzMDJlMzAzYjMyMzkzYkQvTFpOU21iWHZHNWNoU2pKcTZaZFdPMjE2U29raDZnOEZkTTdQbEhhYVk9");

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(Int32.Parse(port));
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add device-specific services used by the ZinzotNet.Shared project
builder.Services.AddSyncfusionBlazor();
builder.Services.AddAntDesign();

builder.Services.AddSingleton<IFormFactor, FormFactor>();
builder.Services.AddScoped<ISupabaseService, SupabaseService>();
builder.Services.AddSingleton<IS3Service, S3Service>();
builder.Services.AddScoped<TableReferenceState>();
builder.Services.AddScoped<DetailReferenceState>();
builder.Services.AddSingleton<SampleService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(ZinzotNet.Shared._Imports).Assembly);

app.Run();

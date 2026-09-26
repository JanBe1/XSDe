using XSDe.Components;
using XSDe.Services.BackgroundServices;
using XSDe.Services.ButtonMappingServices;
using XSDe.Services.Executors;
using XSDe.Services.InputDrivers;
using XSDe.Services.InputDrivers.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddSingleton<IActionExecutor, ActionExecutor>();
builder.Services.AddSingleton<IMappingService, JustAHardcodedListMappingService>();
builder.Services.AddSingleton<IInputDriver, XInputDriver>();
builder.Services.AddHostedService<XInputDeckService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

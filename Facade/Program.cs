using FysioEnterprise.Infrastructure;
using FysioEnterprise.Infrastructure.Database;
using FysioEnterprise.Presentation.Components;
using FysioEnterprise.Presentation.Service;
using FysioEnterprise.UseCase.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using static FysioEnterprise.Infrastructure.Database.SeedData;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services
    .AddInfrastructureServices(builder.Configuration)
    .AddUseCaseServices()
    .AddPresentationServices(builder.Configuration);

var app = builder.Build();


//Seed Data
using(var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDBContext>();
    context.Database.EnsureCreated();
    await context.SeedDataMigrateAsync();
}


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

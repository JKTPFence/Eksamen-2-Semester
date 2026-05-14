using FysioEnterprise.UseCase.IRepositories;
using FysioEnterprise.Presentation.Components.Pages;
using static FysioEnterprise.Infrastructure.Database.SeedData;
using Microsoft.Extensions.DependencyInjection;
using FysioEnterprise.Infrastructure;
using FysioEnterprise.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();


//Seed Data
using(var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDBContext>();
    context.Database.Migrate();

    if (!context.Clinics.Any())
    {
        //Clinics og rooms
        var clinics = ClinicSeed.GetSeedData();
        context.Clinics.AddRange(clinics);
        context.SaveChanges();

        //Staff
        var staff = StaffSeed.GetSeedData(clinics);
        context.Staff.AddRange(staff);
        context.SaveChanges();

        //Clients
        var clients = ClientSeed.GetSeedData(staff);
        context.Clients.AddRange(clients);
        context.SaveChanges();

        //Sessions - henter sessiontype via databasen
        var sessionTypes = context.SessionTypes.ToList();
        var sessions = SessionSeed.GetSeedData(clients, staff, sessionTypes, clinics);
        context.Sessions.AddRange(sessions);
        context.SaveChanges();
    }
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
//builder.Services.AddDomainServices();
//builder.Services.AddApplicationServices();
//builder.Services.AddInfrastructureServices(builder.Configuration);


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

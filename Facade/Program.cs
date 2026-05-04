using FysioEnterprise.Application.Repository.Interfaces;
using FysioEnterprise.Facade.UseCase;
using FysioEnterprise.Infrastructure.Database.Repository;
using FysioEnterprise.Presentation.Components;
using FysioEnterprise.UseCase.Commands;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<ICreateSessionUseCase, SessionCommandHandler>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>();

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
//builder.Services.AddDomainServices();
//builder.Services.AddApplicationServices();
//builder.Services.AddInfrastructureServices(builder.Configuration);


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

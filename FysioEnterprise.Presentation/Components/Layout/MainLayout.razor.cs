using FysioEnterprise.Presentation.Service;
using Microsoft.AspNetCore.Components;

namespace FysioEnterprise.Presentation.Components.Layout
{
    public partial class MainLayout : LayoutComponentBase, IDisposable
    {
        [Inject] private LogInContext context { get; set; } = default!;

        [Inject] private NavigationManager Nav { get; set; } = default!;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await context.LoadFromStorageAsync();
                StateHasChanged();
            }
        }

        protected override void OnInitialized()
        {
            context.OnChange += StateHasChanged;
        }

        public void Dispose()
        {
            context.OnChange -= StateHasChanged;
        }

        private async Task Logout()
        {
            await Context.ClearAsync();
            Nav.NavigateTo("/");
        }
    }
}

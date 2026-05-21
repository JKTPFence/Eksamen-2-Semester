using FysioEnterprise.Presentation.Service;
using Microsoft.AspNetCore.Components;

namespace FysioEnterprise.Presentation.Components.Layout
{
    public partial class MainLayout : LayoutComponentBase, IDisposable
    {
        [Inject] private LogInContext context { get; set; } = default!;

        [Inject] private NavigationManager Nav { get; set; } = default!;
        
        protected override void OnInitialized()
        {
            Context.OnChange += HandleContextChanged;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                try
                {
                    await context.LoadFromStorageAsync();
                }
                catch (InvalidOperationException ex)
                {

                }
            }
        }

        private void HandleContextChanged()
        {
            InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            Context.OnChange -= HandleContextChanged;
        }

        private async Task Logout()
        {
            await Context.ClearAsync();
            Nav.NavigateTo("/", forceLoad: true);
        }
    }
}

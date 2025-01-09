using Microsoft.AspNetCore.Components;

namespace BlazorWebApp.Components.Pages.cascading_parameters
{
    public interface ITab
    {
        RenderFragment ChildContent { get; }
    }
}

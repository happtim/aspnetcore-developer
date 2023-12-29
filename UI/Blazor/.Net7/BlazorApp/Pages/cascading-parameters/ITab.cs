using Microsoft.AspNetCore.Components;

namespace BlazorApp.Pages.cascading_parameters
{
    public interface ITab
    {
        RenderFragment ChildContent { get; }
    }
}

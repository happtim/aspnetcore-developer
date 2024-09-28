using BlazorWebassembly.Pages.skiasharp.Draws;

namespace BlazorWebassembly.Pages.skiasharp.Commands.Edits
{
    public interface IEditOperation
    {
        void Apply(DrawElement element, object newState, object oldState);
        //object GetCurrentState(DrawElement element);
    }
}

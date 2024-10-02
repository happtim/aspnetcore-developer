using BlazorWebassembly.Pages.skiasharp.Draws;

namespace BlazorWebassembly.Pages.skiasharp.Commands
{
    public interface ICommand
    {
        void Execute(DrawingManager drawManager);
        void Undo(DrawingManager drawManager);
    }
}

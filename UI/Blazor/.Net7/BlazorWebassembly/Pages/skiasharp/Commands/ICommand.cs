using BlazorWebassembly.Pages.skiasharp.Draws;

namespace BlazorWebassembly.Pages.skiasharp.Commands
{
    public interface ICommand
    {
        void Execute(DrawManager drawManager);
        void Undo(DrawManager drawManager);
    }
}

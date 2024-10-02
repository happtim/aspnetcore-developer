using BlazorWebassembly.Pages.skiasharp.Draws;

namespace BlazorWebassembly.Pages.skiasharp.Commands
{
    public class MoveElementCommand : ICommand
    {
        private float _dx;
        private float _dy;
        private DrawingElement _element;

        public MoveElementCommand(DrawingElement element, float dx, float dy)
        {
            _dx = dx;
            _dy = dy;
            _element = element;
        }

        public void Execute(DrawingManager drawManager)
        {
            _element.Move(_dx, _dy);
        }

        public void Undo(DrawingManager drawManager)
        {
            _element.Move(-_dx, -_dy);
        }
    }
}

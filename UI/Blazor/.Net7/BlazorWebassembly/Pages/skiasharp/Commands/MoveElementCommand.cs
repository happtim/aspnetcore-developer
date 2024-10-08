﻿using BlazorWebassembly.Pages.skiasharp.Draws;

namespace BlazorWebassembly.Pages.skiasharp.Commands
{
    public class MoveElementCommand : ICommand
    {
        private float _dx;
        private float _dy;
        private DrawElement _element;

        public MoveElementCommand(DrawElement element, float dx, float dy)
        {
            _dx = dx;
            _dy = dy;
            _element = element;
        }

        public void Execute(DrawManager drawManager)
        {
            _element.Move(_dx, _dy);
        }

        public void Undo(DrawManager drawManager)
        {
            _element.Move(-_dx, -_dy);
        }
    }
}

using BlazorWebassembly.Pages.skiasharp.Draws;
using SkiaSharp;

namespace BlazorWebassembly.Pages.skiasharp.Commands.Edits
{
    public class LineStartPointEditOperation : IEditOperation
    {
        public void Apply(DrawingElement element, object newState, object oldState)
        {
            if (element is LineElement line)
            {
                line.Start = (SKPoint)newState;
            }
        }

        //public object GetCurrentState(DrawElement element)
        //{
        //    return (element as LineElement)?.Start;
        //}
    }
}

using BlazorWebassembly.Pages.skiasharp.Draws;
using SkiaSharp;

namespace BlazorWebassembly.Pages.skiasharp.Commands.Edits
{
    public class LineEndPointEditOperation : IEditOperation
    {
        public void Apply(DrawElement element, object newState, object oldState)
        {
            if (element is LineElement line)
            {
                line.End = (SKPoint)newState;
            }
        }

        //public object GetCurrentState(DrawElement element)
        //{
        //    return (element as LineElement)?.End;
        //}
    }
}

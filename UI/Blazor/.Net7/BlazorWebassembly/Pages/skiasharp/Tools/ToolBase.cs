using SkiaSharp;

namespace BlazorWebassembly.Pages.skiasharp.Tools
{
    public abstract class ToolBase : ITool
    {
        public virtual void Activate()
        {
        }

        public virtual void Deactivate()
        {
        }

        public virtual void MouseDown(SKPoint worldPoint) { }

        public virtual void MouseDrag(SKPoint worldPoint) { }

        public virtual void MouseMove(SKPoint worldPoint) { }

        public virtual void MouseUp(SKPoint worldPoint) { }
    }
}

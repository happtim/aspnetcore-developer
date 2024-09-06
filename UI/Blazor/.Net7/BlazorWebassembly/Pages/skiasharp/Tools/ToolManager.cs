using SkiaSharp;

namespace BlazorWebassembly.Pages.skiasharp.Tools
{
    public class ToolManager : ITool
    {
        public ITool? CurrentTool { get; set; }

        public void SetTool(ITool tool)
        {
            CurrentTool = tool;
        }


        public void MouseDown(SKPoint worldPoint)
        {
            if (CurrentTool != null)
            {
                CurrentTool.MouseDown(worldPoint);
            }
        }

        public void MouseMove(SKPoint worldPoint)
        {
            if (CurrentTool != null)
            {
                CurrentTool.MouseMove(worldPoint);
            }
        }

        public void MouseUp(SKPoint worldPoint)
        {
            if (CurrentTool != null)
            {
                CurrentTool.MouseUp(worldPoint);
            }
        }

    }
}

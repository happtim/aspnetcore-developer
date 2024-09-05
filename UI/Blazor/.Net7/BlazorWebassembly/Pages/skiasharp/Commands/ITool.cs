namespace BlazorWebassembly.Pages.skiasharp.Commands
{
    public interface ITool
    {
        void MouseDown(float x, float y);
        void MouseMove(float x, float y);
        void MouseUp(float x, float y);
    }
}

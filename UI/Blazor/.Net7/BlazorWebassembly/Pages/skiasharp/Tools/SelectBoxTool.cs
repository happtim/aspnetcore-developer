using BlazorWebassembly.Pages.skiasharp.Draws;
using SkiaSharp;

namespace BlazorWebassembly.Pages.skiasharp.Tools
{
    public class SelectBoxTool : ToolBase, ITool
    {
        private SKPoint? _start;
        private DrawingManager _drawManager;
        private SelectedManager _selectedManager;

        public SelectBoxTool(DrawingManager drawManager, SelectedManager selectedManager)
        {
            _drawManager = drawManager;
            _selectedManager = selectedManager;
        }

        public void MouseDown(SKPoint worldPoint)
        {
            _start = worldPoint;
        }

        public void MouseDrag(SKPoint worldPoint)
        {
            var end = worldPoint;

            var selectBox = new SelectBoxElement(_start.Value, end);
            _drawManager.SetPreviewElement(selectBox);

            _drawManager.Invalidate(); // 触发重绘
        }

        public void MouseUp(SKPoint worldPoint)
        {
            SKPoint end = worldPoint;

            // 创建选择框的矩形  
            SKRect selectionRect = SKRect.Create(
                Math.Min(_start.Value.X, end.X),
                Math.Min(_start.Value.Y, end.Y),
                Math.Abs(end.X - _start.Value.X),
                Math.Abs(end.Y - _start.Value.Y)
            );

            // 获取所有被选择框包含的元素  
            List<DrawingElement> selectedElements = _drawManager.GetElementsInRect(selectionRect);

            // 更新选中管理器  
            if (selectedElements.Count > 0)
            {
                _selectedManager.AddRange(selectedElements);
            }
            else
            {
                _selectedManager.Clear();
            }

            // 清理临时变量和预览元素
            _drawManager.SetPreviewElement(null);
            _drawManager.Invalidate();
        }
    }
}

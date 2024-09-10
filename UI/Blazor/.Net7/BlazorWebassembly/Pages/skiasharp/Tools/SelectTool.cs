using BlazorWebassembly.Pages.skiasharp.Commands;
using BlazorWebassembly.Pages.skiasharp.Draws;
using SkiaSharp;

namespace BlazorWebassembly.Pages.skiasharp.Tools
{
    public class SelectTool : ITool
    {
        private DrawManager _drawManager;
        private SelectedManager _selectedManager;

        private SKPoint? _start;


        public SelectTool(
            DrawManager drawManager, 
            SelectedManager selectedManager)
        {
            _drawManager = drawManager;
            _selectedManager = selectedManager;

        }

        /// <summary>
        /// 1. 点击空白处，清空选中
        /// 2. 点击元素，选中元素
        /// 3. 点击shift，多选
        /// 4. 点击拖动，拉出选择框
        /// </summary>
        /// <param name="worldPoint"></param>
        public void MouseDown(SKPoint worldPoint)
        {
            var item = _drawManager.HitTest(worldPoint);

            if (item == null)
            {
                _selectedManager.Clear();

                _start = worldPoint;
            }
            else
            {
                _selectedManager.Set(new List<DrawElement> { item});
            }
            
        }

        public void MouseMove(SKPoint worldPoint)
        {

            if (_start != null) 
            {
                var end = worldPoint;

                var selectBox = new SelectBoxElement(_start.Value, end);

                _drawManager.PreviewElement = selectBox;

                _drawManager.Invalidate(); // 触发重绘
            }
            else
            {

                var item = _drawManager.HitTest(worldPoint);

                _selectedManager.SetHover(item);
            }

        }

        public void MouseUp(SKPoint worldPoint)
        {
            if(_start != null)
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
                List<DrawElement> selectedElements = _drawManager.GetElementsInRect(selectionRect);

                // 更新选中管理器  
                if (selectedElements.Count > 0)
                {
                    _selectedManager.Set(selectedElements);
                }
                else
                {
                    _selectedManager.Clear();
                }

                // 清理临时变量和预览元素  
                _start = null;
                _drawManager.PreviewElement = null;
                _drawManager.Invalidate();
            }
        }
    }
}

using BlazorWebassembly.Pages.skiasharp.Commands;
using BlazorWebassembly.Pages.skiasharp.Draws;
using SkiaSharp;

namespace BlazorWebassembly.Pages.skiasharp.Tools
{
    public class SelectTool : ITool
    {
        private DrawManager _drawManager;
        private SelectedManager _selectedManager;
        private ToolManager _toolManager;
        private CommandManager _commandManager;

        private ITool _innerTool;

        private SKPoint? _start;


        public SelectTool(
            DrawManager drawManager, 
            SelectedManager selectedManager,
            ToolManager toolManager,
            CommandManager commandManager)
        {
            _drawManager = drawManager;
            _selectedManager = selectedManager;
            _toolManager = toolManager;
            _commandManager = commandManager;
        }

        /// <summary>
        /// 1. 点击空白处，清空选中
        /// 2. 点击元素，选中元素
        /// 3. 点击shift，Toggle选中状态
        /// 4. 点击空白拖动，拉出选择框
        /// 5. 点击选中元素拖动，移动元素
        /// </summary>
        /// <param name="worldPoint"></param>
        public void MouseDown(SKPoint worldPoint)
        {
            var item = _drawManager.HitTest(worldPoint);

            //点击空白处，清空选中
            if (item == null)
            {
                _selectedManager.Clear();

                _start = worldPoint;
            }
            else
            {
                //在选中元素上点击

                if (!_selectedManager.Contains(item)) 
                {
                    _selectedManager.Clear();
                }

                _selectedManager.Add(item);


                var moveTool = new MoveTool(_selectedManager.GetSelected(), _drawManager, _toolManager, _commandManager);
                moveTool.MouseDown(worldPoint);
                _innerTool = moveTool;

            }
            
        }

        public void MouseMove(SKPoint worldPoint)
        {
            //矩形选择框
            if (_start != null)
            {
                var end = worldPoint;

                var selectBox = new SelectBoxElement(_start.Value, end);

                _drawManager.PreviewElement = selectBox;

                _drawManager.Invalidate(); // 触发重绘
            }
            else
            {
                //点击元素拖动
                if (_innerTool != null)
                {
                    _innerTool.MouseMove(worldPoint);
                }
                //鼠标移动到元素上
                else
                {

                    var item = _drawManager.HitTest(worldPoint);

                    _selectedManager.SetHover(item);
                }

            }

        }

        public void MouseUp(SKPoint worldPoint)
        {
            //矩形选择框
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
                    _selectedManager.AddRange(selectedElements);
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
            else
            {
                if ( _innerTool != null)
                {
                    _innerTool.MouseUp(worldPoint);
                }
            }
        }
    }
}

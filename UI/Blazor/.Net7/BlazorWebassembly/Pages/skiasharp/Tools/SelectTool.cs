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
            _start = worldPoint;

            //点击空白处，清空选中, 设置SelectBoxTool
            if (item == null)
            {
                _selectedManager.Clear();

                _innerTool = new SelectBoxTool(_drawManager,_selectedManager);
            }
            else
            {
                _innerTool = null;

                //在选中元素上点击
                if (!_selectedManager.Contains(item)) 
                {
                    _selectedManager.Clear();
                }

                _selectedManager.Add(item);
            }

            _innerTool?.MouseDown(worldPoint);
            
        }

        public void MouseDrag(SKPoint worldPoint)
        {
 
            if (_innerTool == null && _start != null) 
            {
                var moveTool = new MoveTool(_selectedManager.GetSelected(), _drawManager, _toolManager, _commandManager);
                moveTool.MouseDown(_start.Value);
                _innerTool = moveTool;
            }

            _innerTool?.MouseDrag(worldPoint);

            Console.WriteLine("Current Tool:" + _innerTool?.GetType().Name);

        }

        public void MouseMove(SKPoint worldPoint)
        {
            //鼠标移动到元素上高亮显示
            var item = _drawManager.HitTest(worldPoint);

            _selectedManager.SetHover(item);

        }

        public void MouseUp(SKPoint worldPoint)
        {
            _innerTool?.MouseUp(worldPoint);
        }
    }
}

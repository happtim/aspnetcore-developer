using BlazorWebassembly.Pages.skiasharp.Commands;
using BlazorWebassembly.Pages.skiasharp.Draws;
using SkiaSharp;

namespace BlazorWebassembly.Pages.skiasharp.Tools
{
    public class SelectTool : ToolBase, ITool
    {
        private DrawingManager _drawManager;
        private SelectedManager _selectedManager;
        private ToolManager _toolManager;
        private CommandManager _commandManager;
        private CursorManager _cursorManager;
        private KeyboardManager _keyboardManager;

        private ITool _innerTool;
        private SKPoint? _start;


        public SelectTool(
            DrawingManager drawManager, 
            SelectedManager selectedManager,
            ToolManager toolManager,
            CommandManager commandManager,
            CursorManager cursorManager,
            KeyboardManager keyboardManager)
        {
            _drawManager = drawManager;
            _selectedManager = selectedManager;
            _toolManager = toolManager;
            _commandManager = commandManager;
            _cursorManager = cursorManager;
            _keyboardManager = keyboardManager;
        }

        /// <summary>
        /// 1. 点击空白处
        ///     1.1 清空选中
        ///     1.2 拖动，拉出选择框
        /// 2. 点击元素
        ///     2.1 在选中元素上点击，不清空选中
        ///     2.2 在未选中元素上点击，清空选中，选中当前元素
        /// 3. 点击shift，Toggle选中状态
        /// 5. 当选中元素只有一个时，点击控制点，进入编辑模式
        /// </summary>
        /// <param name="worldPoint"></param>
        public void MouseDown(SKPoint worldPoint)
        {

            //如果是Edit模式，检测点击控制点
            if (_selectedManager.ClickedElement != null) 
            {
                var drawElement = _selectedManager.ClickedElement;

                var controlPointIndex = drawElement.GetControlPointIndex(worldPoint);

                if (controlPointIndex != -1)
                {

                    _innerTool = new SelectEditTool(
                       drawElement,
                       _commandManager,
                       _drawManager);

                    _innerTool.MouseDown(worldPoint);
                    return;
                }
            }

            var item = _drawManager.HitTest(worldPoint);

            _selectedManager.SetClickedElement(item);

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

                //如果按下shift，Toggle选中状态
                if (_keyboardManager.IsShiftPressed)
                {
                    if (_selectedManager.Contains(item))
                    {
                        _selectedManager.Remove(item);
                    }
                    else
                    {
                        _selectedManager.Add(item);
                    }
                }
                else
                {
                    //在选中元素上点击
                    if (!_selectedManager.Contains(item))
                    {
                        _selectedManager.Clear();
                    }

                    _selectedManager.Add(item);
                }
    
            }

            _innerTool?.MouseDown(worldPoint);
            
        }

        public void MouseDrag(SKPoint worldPoint)
        {
 
            if (_innerTool == null && _start != null) 
            {
                _innerTool = new SelectMoveTool(_selectedManager, _drawManager, _commandManager);
                _innerTool.MouseDown(_start.Value);
            }

            _innerTool?.MouseDrag(worldPoint);

        }

        public void MouseMove(SKPoint worldPoint)
        {
            int index = -1;
            if (_selectedManager.ClickedElement is var drawElement && drawElement != null)
            {
                index = drawElement.GetControlPointIndex(worldPoint);
                if (drawElement.SetHoverControlPointIndex(index, _cursorManager))
                {
                    _drawManager.Invalidate();
                }

            }

            if(index == -1)
            {
                //鼠标移动到元素上高亮显示
                var item = _drawManager.HitTest(worldPoint);

                _drawManager.SetHoverElement(item);
            }
   

            _innerTool?.MouseMove(worldPoint);

        }

        public void MouseUp(SKPoint worldPoint)
        {
            _innerTool?.MouseUp(worldPoint);
        }
    }
}

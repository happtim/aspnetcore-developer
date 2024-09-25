using BlazorWebassembly.Pages.skiasharp.Commands;
using BlazorWebassembly.Pages.skiasharp.Draws;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using SkiaSharp;

namespace BlazorWebassembly.Pages.skiasharp.Tools
{
    public class ToolManager
    {
        public ITool? _currentTool = null;

        public ITool? _panTool = null;

        //鼠标拖动事件
        private bool _isMouseDown = false;
        private bool _isDragging = false;
        private double _startX;
        private double _startY;
        private const int DragThreshold = 5; // 像素阈值 

        private ViewportManager _viewportManager;
        private DrawManager _drawManager;
        private CommandManager _commandManager;
        private CursorManager _cursorManager;
        private SelectedManager _selectedManager;
        private KeyboardManager _keyboardManager;

        public ToolManager(
            ViewportManager viewportManager, 
            DrawManager drawManager, 
            CommandManager commandManager,
            CursorManager cursorManager,
            SelectedManager selectedManager,
            KeyboardManager keyboardManager
            )
        {
            _viewportManager = viewportManager;
            _drawManager = drawManager;
            _commandManager = commandManager;
            _cursorManager = cursorManager;
            _selectedManager = selectedManager;
            _keyboardManager = keyboardManager;

            SetTool(new SelectTool(
                _drawManager, 
                _selectedManager,
                this,
                _commandManager,
                _cursorManager,
                _keyboardManager
            ));
        }

        public void SetTool(ITool tool)
        {
            if (_currentTool != null) 
            {
                _currentTool.Deactivate();
            }

            _currentTool = tool;

            if (_currentTool != null)
            {
                _currentTool.Activate();
            }

        }


        public async Task MouseDown(MouseEventArgs e)
        {
            var screenPoint = new SKPoint((float)e.OffsetX, (float)e.OffsetY);

            var worldPoint = _viewportManager.ScreenToWorld(screenPoint);

            _isMouseDown = true;
            _startX = e.ClientX;
            _startY = e.ClientY;

            //button middle
            if (e.Button == 1)
            {
                _panTool = new PanTool(_drawManager, this, _viewportManager,_cursorManager);
                _panTool.MouseDown(worldPoint);
            }
            //not middle button
            else if (_currentTool != null)
            {
                _currentTool.MouseDown(worldPoint);
            }


        }

        public async Task MouseMove(MouseEventArgs e)
        {
            var screenPoint = new SKPoint((float)e.OffsetX, (float)e.OffsetY);

            var worldPoint = _viewportManager.ScreenToWorld(screenPoint);

            //打印坐标
            //Console.WriteLine($"mouse x:{e.OffsetX} y:{e.OffsetY} . world x:{woldPoint.X} y:{woldPoint.Y}");
            
            _panTool?.MouseMove(worldPoint);

            if (this._currentTool != null)
            {
                //判断按下，与当前坐标的阈值，如果超过阈值，触发拖动事件
                if (_isMouseDown)
                {
                    var deltaX = e.ClientX - _startX;
                    var deltaY = e.ClientY - _startY;
                    var distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

                    if (distance >= DragThreshold || _isDragging)
                    {
                        _currentTool.MouseDrag(worldPoint);
                        _isDragging = true;
                    }
                }
                else
                {
                    _currentTool.MouseMove(worldPoint);
                }
            }
        }

        public Task MouseUp(MouseEventArgs e)
        {
            var screenPoint = new SKPoint((float)e.OffsetX, (float)e.OffsetY);

            var worldPoint = _viewportManager.ScreenToWorld(screenPoint);

            _isMouseDown = false;
            _isDragging = false;

            //button middle
            if (e.Button == 1) 
            {
                _panTool?.MouseUp(worldPoint);
                _panTool = null;
            }
            //not middle button
            else
            {
                if (_currentTool != null)
                {
                    _currentTool.MouseUp(worldPoint);
                }

                //默认工具 选择工具
                if (_currentTool == null)
                {
                    SetTool(new SelectTool(
                        _drawManager,
                        _selectedManager,
                        this,
                        _commandManager,
                        _cursorManager,
                        _keyboardManager
                        ));
                }
            }

            return Task.CompletedTask;
        }

    }
}

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

        private ViewportManager _viewportManager;
        private DrawManager _drawManager;
        private CommandManager _commandManager;
        private CursorManager _cursorManager;
        private SelectedManager _selectedManager;

        public ToolManager(
            ViewportManager viewportManager, 
            DrawManager drawManager, 
            CommandManager commandManager,
            CursorManager cursorManager,
            SelectedManager selectedManager
            )
        {
            _viewportManager = viewportManager;
            _drawManager = drawManager;
            _commandManager = commandManager;
            _cursorManager = cursorManager;
            _selectedManager = selectedManager;

            _currentTool = new SelectTool(_drawManager, _selectedManager);
        }

        public void SetTool(ITool tool)
        {
            _currentTool = tool;
        }


        public async Task MouseDown(MouseEventArgs e)
        {
            var screenPoint = new SKPoint((float)e.OffsetX, (float)e.OffsetY);

            var worldPoint = _viewportManager.ScreenToWorld(screenPoint);

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
                _currentTool.MouseMove(worldPoint);
            }
        }

        public Task MouseUp(MouseEventArgs e)
        {
            var screenPoint = new SKPoint((float)e.OffsetX, (float)e.OffsetY);

            var worldPoint = _viewportManager.ScreenToWorld(screenPoint);

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
                    SetTool(new SelectTool(_drawManager, _selectedManager));
                }
            }

            return Task.CompletedTask;
        }

    }
}

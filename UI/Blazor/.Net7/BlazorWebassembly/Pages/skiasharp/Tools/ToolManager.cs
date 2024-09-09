using BlazorWebassembly.Pages.skiasharp.Commands;
using BlazorWebassembly.Pages.skiasharp.Draws;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using SkiaSharp;

namespace BlazorWebassembly.Pages.skiasharp.Tools
{
    public class ToolManager
    {
        public ITool? CurrentTool { get; set; }

        public ITool? _panTool = null;

        private ViewportManager _viewportManager;
        private DrawManager _drawManager;
        private CommandManager _commandManager;
        private CursorManager _cursorManager;
        public ToolManager(
            ViewportManager viewportManager, 
            DrawManager drawManager, 
            CommandManager commandManager,
            CursorManager cursorManager)
        {
            _viewportManager = viewportManager;
            _drawManager = drawManager;
            _commandManager = commandManager;
            _cursorManager = cursorManager;
        }

        public void SetTool(ITool tool)
        {
            CurrentTool = tool;
        }


        public async void MouseDown(MouseEventArgs e)
        {
            var screenPoint = new SKPoint((float)e.OffsetX, (float)e.OffsetY);

            var worldPoint = _viewportManager.ScreenToWorld(screenPoint);

            
            //button middle
            if (e.Button == 1 && CurrentTool == null)
            {
                this._panTool = new PanTool(_drawManager, this, _viewportManager,_cursorManager);
                this._panTool.MouseDown(worldPoint);
            }

            if (this.CurrentTool != null)
            {
                CurrentTool.MouseDown(worldPoint);
            }
            else
            {
                if (_drawManager.HitTest(worldPoint) != null)
                {
                    this.SetTool(new MoveTool(_drawManager, this, _commandManager));
                    CurrentTool.MouseDown(worldPoint);
                }
            }

        }

        public async void MouseMove(MouseEventArgs e)
        {
            var screenPoint = new SKPoint((float)e.OffsetX, (float)e.OffsetY);

            var worldPoint = _viewportManager.ScreenToWorld(screenPoint);

            //打印坐标
            //Console.WriteLine($"mouse x:{e.OffsetX} y:{e.OffsetY} . world x:{woldPoint.X} y:{woldPoint.Y}");
            
            _panTool?.MouseMove(worldPoint);

            if (this.CurrentTool != null)
            {
                CurrentTool.MouseMove(worldPoint);
            }
            else
            {
                if (_drawManager.HitTest(worldPoint) != null)
                {
                    await _cursorManager.SetMove();
                }
                else
                {
                    await _cursorManager.SetDefault();
                }

            }
        }

        public void MouseUp(MouseEventArgs e)
        {
            var screenPoint = new SKPoint((float)e.OffsetX, (float)e.OffsetY);

            var worldPoint = _viewportManager.ScreenToWorld(screenPoint);

            _panTool?.MouseUp(worldPoint);
            _panTool = null;

            if (CurrentTool != null)
            {
                CurrentTool.MouseUp(worldPoint);
            }
        }

    }
}

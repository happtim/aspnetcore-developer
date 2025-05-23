﻿using BlazorWebassembly.Pages.skiasharp.Commands;
using BlazorWebassembly.Pages.skiasharp.Draws;
using SkiaSharp;

namespace BlazorWebassembly.Pages.skiasharp.Tools
{
    public class LineTool : ToolBase, ITool
    {
        private SKPoint _start;
        private SKPoint _end;
        private bool _isDrawing;
        private DrawingManager _drawManager;
        private ToolManager _toolManager;
        private CommandManager _commandManager;
        private CursorManager _cursorManager;

        public LineTool(
            DrawingManager drawManager,
            ToolManager toolManager, 
            CommandManager commandManager,
            CursorManager cursorManager)
        {
            _drawManager = drawManager;
            _toolManager = toolManager;
            _commandManager = commandManager;
            _cursorManager = cursorManager;
        }

        public override void MouseDown(SKPoint worldPoint)
        {

            if (_isDrawing)
            {
                //绘制结束 保存终点
                _end = worldPoint;

                _commandManager.AddCommand(new DrawElementCommand(new LineElement(_start, _end, SKColors.Black)));

                _isDrawing = false;

                _toolManager.SetTool(null);

                _drawManager.SetPreviewElement(null);

                _cursorManager.SetDefault();
            }
            else
            {
                //绘制开始 保存起点

                _start = worldPoint;

                _isDrawing = true;

                _cursorManager.SetCrosshair();

            }


        }


        public override void MouseMove(SKPoint worldPoint)
        {
            if (_isDrawing)
            {
                _end = worldPoint;

                var line = new LineElement(_start, _end, SKColors.Black);

                _drawManager.SetPreviewElement(line);

                _drawManager.Invalidate(); // 触发重绘
            }
        }

    }
}

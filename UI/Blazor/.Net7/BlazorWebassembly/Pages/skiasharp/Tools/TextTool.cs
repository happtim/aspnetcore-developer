using BlazorWebassembly.Pages.skiasharp.Commands;
using BlazorWebassembly.Pages.skiasharp.Draws;
using Microsoft.AspNetCore.Components;
using SkiaSharp;

namespace BlazorWebassembly.Pages.skiasharp.Tools
{
    public class TextTool : ToolBase, ITool
    {
        private TextEdit _textEdit;
        private ViewportManager _viewportManager;
        private CommandManager _commandManager;
        private ToolManager _toolManager;
        private FontManager _fontManager;
        private SKPoint _start;

        public TextTool(TextEdit textEdit, 
            ViewportManager viewportManager,
            CommandManager commandManager,
            ToolManager toolManager,
            FontManager fontManager)
        {
            _commandManager = commandManager;
            _textEdit = textEdit;
            _viewportManager = viewportManager;
            _toolManager = toolManager;

            _textEdit.OnEditorDeactivated = OnEditorDeactivated;
            _toolManager = toolManager;
            _fontManager = fontManager;
        }

        public void MouseDown(SKPoint worldPoint)
        {
            if (_textEdit.IsActivated) 
            {
                return;
            }

            _start = worldPoint;

            var screenPoint = _viewportManager.WorldToScreen(worldPoint);

            _textEdit.ShowEditor(screenPoint.X, screenPoint.Y);
        }

        private async void OnEditorDeactivated()
        {

            var text =  _textEdit.Text;

            var font = await _fontManager.GetFontAsync("NotoSansSC","");

            if (!string.IsNullOrEmpty(text)) 
            {
                _commandManager.AddCommand(new DrawElementCommand(new TextElement(_start, text, SKColors.Black, font)));
            }

            _toolManager.SetTool(null);

            // 例如，你可以在这里调用 Deactivate 方法  
            _textEdit.HideEditor();
        }
    }
}

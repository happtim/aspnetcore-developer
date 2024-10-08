﻿using BlazorWebassembly.Pages.skiasharp.Commands;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorWebassembly.Pages.skiasharp
{
    public class ShortcutManager
    {
        private readonly KeyboardManager _keyboardManager;
        private readonly CommandManager _commandManager;
        private readonly SelectedManager _selectedManager;
        private readonly CopyPasteManager _copyPasteManager;

        public ShortcutManager(
            KeyboardManager keyboardManager,
            CommandManager commandManager,
            SelectedManager selectedManager,
            CopyPasteManager copyPasteManager
            )
        {
            _keyboardManager = keyboardManager;
            _commandManager = commandManager;
            _selectedManager = selectedManager;
            _copyPasteManager = copyPasteManager;
        }

        public void RegisterShortcuts()
        {
            _keyboardManager.RegisterHandler("delete", HandleDelete);
            _keyboardManager.RegisterHandler("ctrl+z", HandleUndo);
            _keyboardManager.RegisterHandler("ctrl+y", HandleRedo);
            _keyboardManager.RegisterHandler("ctrl+c", HandleCopy);
            _keyboardManager.RegisterHandler("ctrl+v", HandlePaste);
        }

        public void UnregisterShortcuts()
        {
            _keyboardManager.RegisterHandler("delete", HandleDelete);
            _keyboardManager.RegisterHandler("ctrl+z", HandleUndo);
            _keyboardManager.RegisterHandler("ctrl+y", HandleRedo);
            _keyboardManager.RegisterHandler("ctrl+c", HandleCopy);
            _keyboardManager.RegisterHandler("ctrl+v", HandlePaste);
        }

        private Task HandleDelete(KeyboardEventArgs e)
        {
            // 在这里实现 Delete 键的处理逻辑
            // 例如，删除选中的元素
            if (_selectedManager.Count() > 0)
            {
                var deleteCommand = new DeleteElementCommand(_selectedManager.GetSelectedElements().ToList());
                _commandManager.AddCommand(deleteCommand);
                _selectedManager.Clear();
            }

            return Task.CompletedTask;
        }

        private Task HandleUndo(KeyboardEventArgs e)
        {
            _commandManager.Undo();
            return Task.CompletedTask;
        }

        private Task HandleRedo(KeyboardEventArgs e)
        {
            _commandManager.Redo();
            return Task.CompletedTask;
        }

        private Task HandleCopy(KeyboardEventArgs e)
        {
            _copyPasteManager.Copy();
            return Task.CompletedTask;
        }


        private Task HandlePaste(KeyboardEventArgs e)
        {
            _copyPasteManager.Paste();
            return Task.CompletedTask;
        }
    }
}

using Microsoft.AspNetCore.Components.Web;

namespace BlazorWebassembly.Pages.skiasharp
{
    public class KeyboardManager
    {
        // 定义按键事件的委托  
        public delegate Task KeyHandler(KeyboardEventArgs e);

        // 使用字典存储不同按键对应的处理器  
        private readonly Dictionary<string, List<KeyHandler>> _keyHandlers = new();

        private readonly HashSet<string> _pressedKeys = new();

        // 修饰键状态  
        public bool IsCtrlPressed { get; private set; }
        public bool IsShiftPressed { get; private set; }
        public bool IsAltPressed { get; private set; }
        public bool IsMetaPressed { get; private set; }
        public bool IsAnyModifierPressed => IsCtrlPressed || IsShiftPressed || IsAltPressed || IsMetaPressed;


        // 注册按键处理器  
        public void RegisterHandler(string key, KeyHandler handler)
        {
            if (!_keyHandlers.ContainsKey(key))
            {
                _keyHandlers[key] = new List<KeyHandler>();
            }
            _keyHandlers[key].Add(handler);
        }

        // 注销按键处理器  
        public void UnregisterHandler(string key, KeyHandler handler)
        {
            if (_keyHandlers.ContainsKey(key))
            {
                _keyHandlers[key].Remove(handler);
                if (_keyHandlers[key].Count == 0)
                {
                    _keyHandlers.Remove(key);
                }
            }
        }

        // 处理按键事件  
        public async Task HandleKeyDown(KeyboardEventArgs e)
        {
            UpdateModifierKeys(e.Key, isKeyDown: true);

            var keyCombination = GetKeyCombination(e);
            var keyLower = e.Key.ToLower();

            // 检查键是否已经处于按下状态  
            if (!_pressedKeys.Contains(keyLower))
            {
                _pressedKeys.Add(keyLower);

                if (_keyHandlers.TryGetValue(keyCombination, out var handlers))
                {
                    foreach (var handler in handlers)
                    {
                        await handler.Invoke(e);
                    }
                }

                // 处理单独按键的事件  
                if (_keyHandlers.TryGetValue(keyLower, out var singleHandlers))
                {
                    foreach (var handler in singleHandlers)
                    {
                        await handler.Invoke(e);
                    }
                }
            }

        }

        // 处理键盘释放事件  
        public void HandleKeyUp(KeyboardEventArgs e)
        {
            _pressedKeys.Remove(e.Key.ToLower());

            UpdateModifierKeys(e.Key, isKeyDown: false);
        }

        // 更新修饰键的状态  
        private void UpdateModifierKeys(string key, bool isKeyDown)
        {
            switch (key.ToLower())
            {
                case "control":
                case "ctrl":
                    IsCtrlPressed = isKeyDown;
                    break;
                case "shift":
                    IsShiftPressed = isKeyDown;
                    break;
                case "alt":
                    IsAltPressed = isKeyDown;
                    break;
                case "meta":
                case "cmd":
                case "command":
                    IsMetaPressed = isKeyDown;
                    break;
            }
        }

        // 构建当前按键的组合字符串，例如 "ctrl+c"  
        private string GetKeyCombination(KeyboardEventArgs e)
        {
            List<string> parts = new();

            if (e.CtrlKey)
                parts.Add("ctrl");
            if (e.ShiftKey)
                parts.Add("shift");
            if (e.AltKey)
                parts.Add("alt");
            if (e.MetaKey)
                parts.Add("meta");

            // 特殊处理区分大小写的键（例如字母）  
            string key = e.Key.ToLower();
            // 忽略修饰键的名称  
            if (key == "control" || key == "shift" || key == "alt" || key == "meta")
                return string.Join("+", parts);

            parts.Add(key);
            return string.Join("+", parts);
        }

    }
}

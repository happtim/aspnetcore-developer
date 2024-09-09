using Microsoft.JSInterop;

namespace BlazorWebassembly.Pages.skiasharp
{
    public class CursorManager
    {
        private string _cursor = "default";
        private IJSRuntime _jsRuntime;
        public CursorManager(IJSRuntime jsRuntime) 
        {
            _jsRuntime = jsRuntime;
        }

        public async Task SetCursor(string cursor)
        {
            if (_cursor != cursor)
            {
                _cursor = cursor;
                await _jsRuntime.InvokeVoidAsync("changeCursor", cursor);
            }
        }

        public async Task SetDefault()
        {
            await SetCursor("default");
        }
        public async Task SetMove()
        {
            await SetCursor("move");
        }

        public Task SetHand()
        {
            //await SetCursor("pointer");
            return Task.CompletedTask;
        }


    }
}

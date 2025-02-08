using Microsoft.JSInterop;

namespace BlazorWebLLM.Client
{
    //该服务将为 Blazor 应用中的各个组件与 JavaScript 模块之间提供一个抽象层。
    public class WebLLMService
    {
        private readonly Lazy<Task<IJSObjectReference>> moduleTask;

        private const string ModulePath = "./webllm-interop2.js";

        public WebLLMService(IJSRuntime jsRuntime)
        {
            moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", $"{ModulePath}").AsTask());
        }

        private string selectedModel = "Llama-3.2-1B-Instruct-q4f16_1-MLC";

        public async Task InitializeAsync()
        {
   	        var module = await moduleTask.Value;
   	        await module.InvokeVoidAsync("initialize", selectedModel, DotNetObjectReference.Create(this));
            // Calls webllm-interop.js    initialize  (selectedModel, dotnet                            ) 
        }

        public event Action<InitProgress>? OnInitializingChanged;

        // Called from JavaScript
        // dotnetInstance.invokeMethodAsync("OnInitializing", initProgress);
        [JSInvokable]
        public Task OnInitializing(InitProgress status)
        {
            OnInitializingChanged?.Invoke(status);
            return Task.CompletedTask;
        }

         public async Task CompleteStreamAsync(IList<Message> messages)
         {
 	        var module = await moduleTask.Value;
 	        await module.InvokeVoidAsync("completeStream", messages);
         }
 
         public event Func<WebLLMCompletion, Task>? OnChunkCompletion;
 
         [JSInvokable]
         public Task ReceiveChunkCompletion(WebLLMCompletion response)
         {
 	        OnChunkCompletion?.Invoke(response);
 	        return Task.CompletedTask;
         }


    }
}

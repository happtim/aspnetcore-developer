//不是增加 npm 的复杂性，我们将直接从 CDN 获取模块。
import * as webllm from "https://esm.run/@mlc-ai/web-llm";

var engine; // <-- hold a reference to MLCEngine in the module
var dotnetInstance; // <-- hold a reference to the WebLLMService instance in the module

// 当调用 CreateMLCEngine 时，使用回调来捕获模型加载进度。
const initProgressCallback = (initProgress) => {
    // Make a call to.NET with the updated status
    dotnetInstance.invokeMethodAsync("OnInitializing", initProgress);

}

//WebLLM 通过创建一个 engine 的实例进行初始化。
export async function initialize(selectedModel, dotnet) {
    dotnetInstance = dotnet; // <-- WebLLMService insntance
        engine = await webllm.CreateMLCEngine(
        selectedModel,
        { initProgressCallback: initProgressCallback }, // engineConfig
    );
}

export async function completeStream(messages) {
    // Chunks is an AsyncGenerator object
    const chunks = await engine.chat.completions.create({
        messages,
        temperature: 1,
        stream: true, // <-- Enable streaming
        stream_options: { include_usage: true },
    });

    for await (const chunk of chunks) {
        //console.log(chunk);
        await dotnetInstance.invokeMethodAsync("ReceiveChunkCompletion", chunk);
    }
 }
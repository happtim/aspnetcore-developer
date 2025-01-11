//不是增加 npm 的复杂性，我们将直接从 CDN 获取模块。
import * as webllm from "https://esm.run/@mlc-ai/web-llm";

// 当调用 CreateMLCEngine 时，使用回调来捕获模型加载进度。
const initProgressCallback = (initProgress) => {
    console.log(initProgress);
    //{
    //    progress: 0.024131450420356197
    //    text: "Fetching param cache[0/22]: 16MB fetched. 2% completed, 7 secs elapsed. It can take a while when we first visit this page to populate the cache. Later refreshes will become faster."
    //    timeElapsed: 7
    //}

}
//当前的 selectedModel ， Llama-3.2-1B-Instruct-q4f16_1-MLC ，是一个较小的模型，加载时间会更少。
//请注意，一些模型可能需要几 GB 的数据传输。
const selectedModel = "Llama-3.2-1B-Instruct-q4f16_1-MLC";

//WebLLM 通过创建一个 engine 的实例进行初始化。
const engine = await webllm.CreateMLCEngine(
    selectedModel,
    { initProgressCallback: initProgressCallback }, // engineConfig
);
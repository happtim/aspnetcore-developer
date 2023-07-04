<Query Kind="Expression" />

//DotNetty.Common 是公共的类库项目，包装线程池，并行任务和常用帮助类的封装
//DotNetty.Transport 是DotNetty核心的实现
//DotNetty.Buffers 是对内存缓冲区管理的封装
//DotNetty.Codes 是对编码器解码器的封装，包括一些基础基类的实现，我们在项目中自定义的协议，都要继承该项目的特定基类和实现
//DotNetty.Handlers 封装了常用的管道处理器，比如Tls编解码，超时机制，心跳检查，日志等，如果项目中没有用到可以不引用，不过一般都会用到


//Channel
//Channel是Socket的封装，提供绑定，读，写等操作，降低了直接使用Socket的复杂性。

//EventLoop
//一个 EventLoopGroup 包含一个或者多个 EventLoop；
//一个 EventLoop 在它的生命周期内只和一个 Thread 绑定；
//所有由 EventLoop 处理的 I/ O 事件都将在它专有的 Thread 上被处理；
//一个 Channel 在它的生命周期内只注册于一个 EventLoop；
//一个 EventLoop 可能会被分配给一个或多个 Channel。


//ChannelHandler
//ChannelHandler是处理数据出入站事件的逻辑容器，可以处理入站数据以及给客户端以回复。

//ChannelPipeline
//ChannelPipeline是将ChannelHandler穿成一串的的容器。

//Bootstrap引导类
//Bootstrap用于引导客户端，ServerBootstrap用于引导服务器
//客户端引导类只需要一个EventLoopGroup服务器引导类需要两个EventLoopGroup。但是在简单使用中，也可以公用一个EventLoopGroup。

//为什么服务器需要两个EventLoopGroup呢？是因为服务器的第一个EventLoopGroup只有一个EventLoop，
//只含有一个SeverChannel用于监听本地端口，一旦连接建立，这个EventLoop就将Channel控制权移交给另一个EventLoopGroup，
//这个EventLoopGroup分配一个EventLoop给Channel用于管理这个Channel。

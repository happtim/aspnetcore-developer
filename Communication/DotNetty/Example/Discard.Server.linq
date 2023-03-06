<Query Kind="Statements">
  <NuGetReference>DotNetty.Handlers</NuGetReference>
  <NuGetReference>DotNetty.Transport</NuGetReference>
  <NuGetReference Version="6.0.0">Microsoft.Extensions.Logging.Console</NuGetReference>
  <Namespace>DotNetty.Handlers.Logging</Namespace>
  <Namespace>DotNetty.Transport.Bootstrapping</Namespace>
  <Namespace>DotNetty.Transport.Channels</Namespace>
  <Namespace>DotNetty.Transport.Channels.Sockets</Namespace>
  <Namespace>Microsoft.Extensions.Logging</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>DotNetty.Common.Internal.Logging</Namespace>
</Query>


//https://github.com/netty/netty/wiki/User-guide-for-4.x#writing-a-discard-server
//这个DisCard协议丢弃所有的收到消息，没有任何回复

InternalLoggerFactory.DefaultFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug));

//MultithreadEventLoopGroup 是一个处理 I/O 操作的多线程事件循环
//第一个，通常称为“boss”，接受传入连接主线程只会实例化一个
var bossGroup = new MultithreadEventLoopGroup(1); 
//第二个，通常称为“worker”，一旦“boss”接受连接并将接受的连接注册给worker，就会处理所接受连接的流量。
//子工作组，推荐设置为内核数*2的线程数
var workerGroup = new MultithreadEventLoopGroup();


try
{
	//是设置服务器的帮助程序类。
	var bootstrap = new ServerBootstrap();
	bootstrap
		.Group(bossGroup, workerGroup)
		.Channel<TcpServerSocketChannel>() //该类用于实例化新通道以接受传入连接。
		.Option(ChannelOption.SoBacklog, 100) 	//您还可以设置特定于实现的参数。 如：tcpNoDelay ，keepAlive
		.Handler(new LoggingHandler("LSTN")) ////初始化日志拦截器
		.ChildHandler(new ActionChannelInitializer<ISocketChannel>(channel =>
		{
			/*
			* 这里主要是配置channel中需要被设置哪些参数，以及channel具体的实现方法内容。
			* channel可以理解为，socket通讯当中客户端和服务端的连接会话，会话内容的处理在channel中实现。
			*/
			IChannelPipeline pipeline = channel.Pipeline;
			pipeline.AddLast(new LoggingHandler("CONN"));
			pipeline.AddLast(new DiscardServerHandler());
		}));

	IChannel bootstrapChannel = await bootstrap.BindAsync(8007);

	Console.ReadLine();

	await bootstrapChannel.CloseAsync();
}
finally
{
	Task.WaitAll(bossGroup.ShutdownGracefullyAsync(), workerGroup.ShutdownGracefullyAsync());
}


public class DiscardServerHandler : SimpleChannelInboundHandler<object>
{
	//当收到消息之后，这个方法就会被调用。收到的类型是一个 PooledHeapByteBuffer
	protected override void ChannelRead0(IChannelHandlerContext context, object message)
	{
		//DotNetty.Buffers.PooledHeapByteBuffer
		// PooledHeapByteBuffer 是一个引用类型，需要显示的使用 Release 方式将其释放，该方法在其父类中。
		message.GetType().Dump();
	}

	//该方法可以捕获handler调用时的异常。
	public override void ExceptionCaught(IChannelHandlerContext ctx, Exception e)
	{
		Console.WriteLine("{0}", e.ToString());
		ctx.CloseAsync();
	}
}
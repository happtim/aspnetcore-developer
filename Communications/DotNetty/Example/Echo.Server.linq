<Query Kind="Statements">
  <NuGetReference>DotNetty.Handlers</NuGetReference>
  <NuGetReference>DotNetty.Transport</NuGetReference>
  <NuGetReference Version="6.0.0">Microsoft.Extensions.Logging.Console</NuGetReference>
  <Namespace>Microsoft.Extensions.Logging</Namespace>
  <Namespace>DotNetty.Common.Internal.Logging</Namespace>
  <Namespace>DotNetty.Transport.Channels</Namespace>
  <Namespace>DotNetty.Transport.Bootstrapping</Namespace>
  <Namespace>DotNetty.Transport.Channels.Sockets</Namespace>
  <Namespace>DotNetty.Handlers.Logging</Namespace>
  <Namespace>DotNetty.Codecs</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>DotNetty.Buffers</Namespace>
</Query>

//https://github.com/netty/netty/wiki/User-guide-for-4.x#writing-an-echo-server
//

InternalLoggerFactory.DefaultFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug));

IEventLoopGroup bossGroup;
IEventLoopGroup workerGroup;

bossGroup = new MultithreadEventLoopGroup(1);
workerGroup = new MultithreadEventLoopGroup();

try
{
	var bootstrap = new ServerBootstrap();
	bootstrap.Group(bossGroup, workerGroup);

	bootstrap.Channel<TcpServerSocketChannel>();
	bootstrap
		//存放已完成三次握手的请求的等待队列的最大长度;
		.Option(ChannelOption.SoBacklog, 100)
		//ByteBuf的分配器(重用缓冲区)大小
		.Option(ChannelOption.Allocator, UnpooledByteBufferAllocator.Default)
		//接收字符的长度
		.Option(ChannelOption.RcvbufAllocator, new FixedRecvByteBufAllocator(1024 * 8))
		//保持长连接
		.ChildOption(ChannelOption.SoKeepalive, true)
		//取消延迟发送
		.ChildOption(ChannelOption.TcpNodelay, true)
		//端口复用
		.ChildOption(ChannelOption.SoReuseport, true)
		.Handler(new LoggingHandler("SRV-LSTN"))
		.ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
		{
			IChannelPipeline pipeline = channel.Pipeline;
			pipeline.AddLast(new LoggingHandler("SRV-CONN"));
			pipeline.AddLast("framing-enc", new LengthFieldPrepender(2));
			pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder(ushort.MaxValue, 0, 2, 0, 2));

			pipeline.AddLast("echo", new EchoServerHandler());
		}));

	IChannel boundChannel = await bootstrap.BindAsync(8007);

	Console.ReadLine();

	await boundChannel.CloseAsync();
}
finally
{
	await Task.WhenAll(
		bossGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)),
		workerGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)));
}

public class EchoServerHandler : ChannelHandlerAdapter
{
	/*
	* Channel的生命周期
	* 1.ChannelRegistered 先注册
	* 2.ChannelActive 再被激活
	* 3.ChannelRead 客户端与服务端建立连接之后的会话（数据交互）
	* 4.ChannelReadComplete 读取客户端发送的消息完成之后
	* error. ExceptionCaught 如果在会话过程当中出现dotnetty框架内部异常都会通过Caught方法返回给开发者
	* 5.ChannelInactive 使当前频道处于未激活状态
	* 6.ChannelUnregistered 取消注册
	*/
	public override void ChannelRead(IChannelHandlerContext context, object message)
	{
		var buffer = message as IByteBuffer;
		if (buffer != null)
		{
			Console.WriteLine("Received from client: " + buffer.ToString(Encoding.UTF8));
		}
		
		//请注意，与示例DisCard中不同，我们没有Release发布收到的消息。因为发送的完，Netty会自动释放他。
		context.WriteAsync(message);
		//WriteAsync 不会将消息写出去。写入内容被缓存起来了，使用Flush方法将数据发出去。writeAndFlush 也可以。
		context.Flush();
	}

	public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();

	public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
	{
		Console.WriteLine("Exception: " + exception);
		context.CloseAsync();
	}
}
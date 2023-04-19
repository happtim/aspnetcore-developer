<Query Kind="Statements">
  <NuGetReference>DotNetty.Handlers</NuGetReference>
  <NuGetReference>DotNetty.Transport</NuGetReference>
  <NuGetReference Version="6.0.0">Microsoft.Extensions.Logging.Console</NuGetReference>
  <Namespace>System.Net</Namespace>
  <Namespace>DotNetty.Transport.Channels</Namespace>
  <Namespace>DotNetty.Transport.Bootstrapping</Namespace>
  <Namespace>DotNetty.Transport.Channels.Sockets</Namespace>
  <Namespace>DotNetty.Handlers.Logging</Namespace>
  <Namespace>DotNetty.Buffers</Namespace>
  <Namespace>DotNetty.Common.Internal.Logging</Namespace>
  <Namespace>Microsoft.Extensions.Logging</Namespace>
  <Namespace>Microsoft.Extensions.Logging.Console</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>



InternalLoggerFactory.DefaultFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug));
//客户端与服务端不同的是，只需要一个主工作组进行工作协调即可不需要创建子线程组
var group = new MultithreadEventLoopGroup();

try
{
	var bootstrap = new Bootstrap();
	bootstrap
		.Group(group)
		.Channel<TcpSocketChannel>()
		.Option(ChannelOption.TcpNodelay, true) ////设置为true的话不允许延迟直接发出，因为dotnetty内部实现中会将消息积累到一定的字节之后才发出。
		.Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
		{
			IChannelPipeline pipeline = channel.Pipeline;
			pipeline.AddLast(new LoggingHandler());
			pipeline.AddLast(new DiscardClientHandler());
		}));

	IChannel bootstrapChannel = await bootstrap.ConnectAsync(new IPEndPoint(IPAddress.Parse("127.0.0.1"),8007));

	Console.ReadLine();

	await bootstrapChannel.CloseAsync();
}
finally
{
	group.ShutdownGracefullyAsync().Wait(1000);
}

public class DiscardClientHandler : SimpleChannelInboundHandler<object>
{
	IChannelHandlerContext ctx;
	byte[] array;

	public override void ChannelActive(IChannelHandlerContext ctx)
	{
		this.array = new byte[256];
		this.ctx = ctx;

		// Send the initial messages.
		this.GenerateTraffic();
	}

	protected override void ChannelRead0(IChannelHandlerContext context, object message)
	{
		// Server is supposed to send nothing, but if it sends something, discard it.
	}

	public override void ExceptionCaught(IChannelHandlerContext ctx, Exception e)
	{
		Console.WriteLine("{0}", e.ToString());
		this.ctx.CloseAsync();
	}

	async void GenerateTraffic()
	{
		try
		{
			IByteBuffer buffer = Unpooled.WrappedBuffer(this.array);
			// Flush the outbound buffer to the socket.
			// Once flushed, generate the same amount of traffic again.
			await this.ctx.WriteAndFlushAsync(buffer);
			await Task.Delay(3000);
			this.GenerateTraffic();
		}
		catch
		{
			await this.ctx.CloseAsync();
		}
	}
}
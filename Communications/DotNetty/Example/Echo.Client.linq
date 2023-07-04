<Query Kind="Statements">
  <NuGetReference>DotNetty.Handlers</NuGetReference>
  <NuGetReference>DotNetty.Transport</NuGetReference>
  <NuGetReference Version="6.0.0">Microsoft.Extensions.Logging.Console</NuGetReference>
  <Namespace>DotNetty.Transport.Bootstrapping</Namespace>
  <Namespace>DotNetty.Transport.Channels.Sockets</Namespace>
  <Namespace>DotNetty.Transport.Channels</Namespace>
  <Namespace>DotNetty.Handlers.Logging</Namespace>
  <Namespace>DotNetty.Codecs</Namespace>
  <Namespace>System.Net</Namespace>
  <Namespace>DotNetty.Buffers</Namespace>
  <Namespace>DotNetty.Common.Internal.Logging</Namespace>
  <Namespace>Microsoft.Extensions.Logging</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>



InternalLoggerFactory.DefaultFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug));


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
			pipeline.AddLast("framing-enc", new LengthFieldPrepender(2));
			pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder(ushort.MaxValue, 0, 2, 0, 2));

			pipeline.AddLast("echo", new EchoClientHandler());
		}));

	IChannel clientChannel = await bootstrap.ConnectAsync(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8007));

	Console.ReadLine();

	await clientChannel.CloseAsync();
}
finally
{
	await group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
}

public class EchoClientHandler : ChannelHandlerAdapter
{
	readonly IByteBuffer initialMessage;

	public EchoClientHandler()
	{
		this.initialMessage = Unpooled.Buffer(256);
		byte[] messageBytes = Encoding.UTF8.GetBytes("Hello world");
		this.initialMessage.WriteBytes(messageBytes);
	}

	public override void ChannelActive(IChannelHandlerContext context) => context.WriteAndFlushAsync(this.initialMessage);

	public override void ChannelRead(IChannelHandlerContext context, object message)
	{
		var byteBuffer = message as IByteBuffer;
		if (byteBuffer != null)
		{
			Console.WriteLine("Received from server: " + byteBuffer.ToString(Encoding.UTF8));
		}
		
		context.WriteAsync(message);
	}

	public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();

	public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
	{
		Console.WriteLine("Exception: " + exception);
		context.CloseAsync();
	}
}
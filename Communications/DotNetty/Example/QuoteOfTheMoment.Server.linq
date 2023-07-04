<Query Kind="Statements">
  <NuGetReference Version="0.7.5">DotNetty.Handlers</NuGetReference>
  <NuGetReference Version="0.7.5">DotNetty.Transport</NuGetReference>
  <NuGetReference Version="6.0.0">Microsoft.Extensions.Logging.Console</NuGetReference>
  <Namespace>DotNetty.Common.Internal.Logging</Namespace>
  <Namespace>Microsoft.Extensions.Logging</Namespace>
  <Namespace>DotNetty.Transport.Channels</Namespace>
  <Namespace>DotNetty.Transport.Bootstrapping</Namespace>
  <Namespace>DotNetty.Transport.Channels.Sockets</Namespace>
  <Namespace>DotNetty.Handlers.Logging</Namespace>
  <Namespace>DotNetty.Buffers</Namespace>
</Query>

//https://github.com/Azure/DotNetty/blob/dev/examples/QuoteOfTheMoment.Server/Program.cs

InternalLoggerFactory.DefaultFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug));

var group = new MultithreadEventLoopGroup();
try
{
	var bootstrap = new Bootstrap();
	bootstrap
		.Group(group)
		.Channel<SocketDatagramChannel>()
		.Option(ChannelOption.SoBroadcast, true)
		.Handler(new LoggingHandler("SRV-LSTN"))
		.Handler(new ActionChannelInitializer<IChannel>(channel =>
		{
			channel.Pipeline.AddLast("Quote", new QuoteOfTheMomentServerHandler());
		}));

	IChannel boundChannel = await bootstrap.BindAsync(8007);
	Console.WriteLine("Press any key to terminate the server.");
	Console.ReadLine();

	await boundChannel.CloseAsync();
}
finally
{
	await group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
}

public class QuoteOfTheMomentServerHandler : SimpleChannelInboundHandler<DatagramPacket>
{
	static readonly Random Random = new Random();

	// Quotes from Mohandas K. Gandhi:
	static readonly string[] Quotes =
	{
			"Where there is love there is life.",
			"First they ignore you, then they laugh at you, then they fight you, then you win.",
			"Be the change you want to see in the world.",
			"The weak can never forgive. Forgiveness is the attribute of the strong.",
		};

	static string NextQuote()
	{
		int quoteId = Random.Next(Quotes.Length);
		return Quotes[quoteId];
	}

	protected override void ChannelRead0(IChannelHandlerContext ctx, DatagramPacket packet)
	{
		Console.WriteLine($"Server Received => {packet}");

		if (!packet.Content.IsReadable())
		{
			return;
		}

		string message = packet.Content.ToString(Encoding.UTF8);
		if (message != "QOTM?")
		{
			return;
		}

		byte[] bytes = Encoding.UTF8.GetBytes("QOTM: " + NextQuote());
		IByteBuffer buffer = Unpooled.WrappedBuffer(bytes);
		ctx.WriteAsync(new DatagramPacket(buffer, packet.Sender));
	}

	public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();

	public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
	{
		Console.WriteLine("Exception: " + exception);
		context.CloseAsync();
	}
}
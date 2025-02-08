<Query Kind="Statements">
  <NuGetReference Version="0.7.5">DotNetty.Handlers</NuGetReference>
  <NuGetReference Version="0.7.5">DotNetty.Transport</NuGetReference>
  <NuGetReference Version="6.0.0">Microsoft.Extensions.Logging.Console</NuGetReference>
  <Namespace>DotNetty.Common.Internal.Logging</Namespace>
  <Namespace>Microsoft.Extensions.Logging</Namespace>
  <Namespace>DotNetty.Transport.Channels</Namespace>
  <Namespace>DotNetty.Transport.Bootstrapping</Namespace>
  <Namespace>DotNetty.Transport.Channels.Sockets</Namespace>
  <Namespace>System.Net</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>DotNetty.Buffers</Namespace>
</Query>

InternalLoggerFactory.DefaultFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug));


var group = new MultithreadEventLoopGroup();

try
{
	var bootstrap = new Bootstrap();
	  bootstrap
        .Group(group)
        .Channel<SocketDatagramChannel>()
        .Option(ChannelOption.SoBroadcast, true)
        .Handler(new ActionChannelInitializer<IChannel>(channel =>
        {
            channel.Pipeline.AddLast("Quote", new QuoteOfTheMomentClientHandler());
        }));

  	IChannel clientChannel = await bootstrap.BindAsync(IPEndPoint.MinPort);

	Console.WriteLine("Sending broadcast QOTM");

    // Broadcast the QOTM request to port.
    byte[] bytes = Encoding.UTF8.GetBytes("QOTM?");
    IByteBuffer buffer = Unpooled.WrappedBuffer(bytes);
    await clientChannel.WriteAndFlushAsync(
        new DatagramPacket(
			buffer,
			new IPEndPoint(IPAddress.Broadcast, 8007)));

	Console.WriteLine("Waiting for response.");

	await Task.Delay(5000);
	Console.WriteLine("Waiting for response time 5000 completed. Closing client channel.");

	await clientChannel.CloseAsync();
}
finally
{
	await group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
}

public class QuoteOfTheMomentClientHandler : SimpleChannelInboundHandler<DatagramPacket>
{
	protected override void ChannelRead0(IChannelHandlerContext ctx, DatagramPacket packet)
	{
		Console.WriteLine($"Client Received => {packet}");

		if (!packet.Content.IsReadable())
		{
			return;
		}

		string message = packet.Content.ToString(Encoding.UTF8);
		if (!message.StartsWith("QOTM: "))
		{
			return;
		}

		Console.WriteLine($"Quote of the Moment: {message.Substring(6)}");
		ctx.CloseAsync();
	}

	public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
	{
		Console.WriteLine("Exception: " + exception);
		context.CloseAsync();
	}
}
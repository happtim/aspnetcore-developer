<Query Kind="Statements">
  <NuGetReference Version="0.7.5">DotNetty.Handlers</NuGetReference>
  <NuGetReference Version="0.7.5">DotNetty.Transport</NuGetReference>
  <NuGetReference Version="6.0.0">Microsoft.Extensions.Logging.Console</NuGetReference>
  <Namespace>DotNetty.Buffers</Namespace>
  <Namespace>DotNetty.Transport.Channels</Namespace>
  <Namespace>DotNetty.Common.Internal.Logging</Namespace>
  <Namespace>DotNetty.Transport.Bootstrapping</Namespace>
  <Namespace>DotNetty.Transport.Channels.Sockets</Namespace>
  <Namespace>DotNetty.Handlers.Logging</Namespace>
  <Namespace>System.Net</Namespace>
  <Namespace>Microsoft.Extensions.Logging</Namespace>
  <Namespace>DotNetty.Codecs</Namespace>
  <Namespace>System.Numerics</Namespace>
  <Namespace>System.Collections.Concurrent</Namespace>
</Query>

#load ".\Factorial"

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

			pipeline.AddLast(new LoggingHandler("CONN"));
			pipeline.AddLast(new BigIntegerDecoder());
			pipeline.AddLast(new NumberEncoder());
			pipeline.AddLast(new FactorialClientHandler());
		}));

	IChannel bootstrapChannel  = await bootstrap.ConnectAsync(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8007));

	// Get the handler instance to retrieve the answer.
	var handler = (FactorialClientHandler)bootstrapChannel.Pipeline.Last();

	// Print out the answer.
	Console.WriteLine("Factorial of {0} is: {1}", "100" , handler.GetFactorial().ToString());

	Console.ReadLine();

	await bootstrapChannel.CloseAsync();
}
finally
{
	await group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
}

public class FactorialClientHandler : SimpleChannelInboundHandler<BigInteger>
{
	IChannelHandlerContext ctx;
	int receivedMessages;
	int next = 1;
	readonly BlockingCollection<BigInteger> answer = new BlockingCollection<BigInteger>();

	public BigInteger GetFactorial() => this.answer.Take();

	public override void ChannelActive(IChannelHandlerContext ctx)
	{
		this.ctx = ctx;
		this.SendNumbers();
	}

	protected override void ChannelRead0(IChannelHandlerContext ctx, BigInteger msg)
	{
		this.receivedMessages++;
		if (this.receivedMessages == 100)
		{
			ctx.CloseAsync().ContinueWith(t => this.answer.Add(msg));
		}
	}

	void SendNumbers()
	{
		for (int i = 0; (i < 4096) && (this.next <= 100); i++)
		{
			this.ctx.WriteAsync(new BigInteger(this.next));
			this.next++;
		}
		this.ctx.Flush();
	}
}


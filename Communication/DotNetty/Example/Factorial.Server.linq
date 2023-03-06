<Query Kind="Statements">
  <NuGetReference>DotNetty.Handlers</NuGetReference>
  <NuGetReference>DotNetty.Transport</NuGetReference>
  <NuGetReference Version="6.0.0">Microsoft.Extensions.Logging.Console</NuGetReference>
  <Namespace>DotNetty.Common.Internal.Logging</Namespace>
  <Namespace>Microsoft.Extensions.Logging</Namespace>
  <Namespace>DotNetty.Transport.Channels</Namespace>
  <Namespace>DotNetty.Transport.Bootstrapping</Namespace>
  <Namespace>DotNetty.Transport.Channels.Sockets</Namespace>
  <Namespace>DotNetty.Buffers</Namespace>
  <Namespace>DotNetty.Handlers.Logging</Namespace>
  <Namespace>DotNetty.Codecs</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Numerics</Namespace>
</Query>

#load ".\Factorial"

//https://github.com/Azure/DotNetty/blob/dev/examples/Factorial.Server/Program.cs

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
		.Handler(new LoggingHandler("LSTN"))
		.ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
		{
			IChannelPipeline pipeline = channel.Pipeline;
			pipeline.AddLast(new LoggingHandler("CONN"));
			pipeline.AddLast(new NumberEncoder(), new BigIntegerDecoder(), new FactorialServerHandler());
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

public class FactorialServerHandler : SimpleChannelInboundHandler<BigInteger>
{
	BigInteger lastMultiplier = new BigInteger(1);
	BigInteger factorial = new BigInteger(1);

	protected override void ChannelRead0(IChannelHandlerContext ctx, BigInteger msg)
	{
		this.lastMultiplier = msg;
		this.factorial *= msg;
		ctx.WriteAndFlushAsync(this.factorial);
	}

	public override void ChannelInactive(IChannelHandlerContext ctx) => Console.WriteLine("Factorial of {0} is: {1}", this.lastMultiplier, this.factorial);

	public override void ExceptionCaught(IChannelHandlerContext ctx, Exception e) => ctx.CloseAsync();
}


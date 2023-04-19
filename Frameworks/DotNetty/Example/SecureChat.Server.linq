<Query Kind="Statements">
  <NuGetReference>DotNetty.Handlers</NuGetReference>
  <NuGetReference>DotNetty.Transport</NuGetReference>
  <NuGetReference Version="6.0.0">Microsoft.Extensions.Logging.Console</NuGetReference>
  <Namespace>DotNetty.Common.Internal.Logging</Namespace>
  <Namespace>Microsoft.Extensions.Logging</Namespace>
  <Namespace>DotNetty.Transport.Channels</Namespace>
  <Namespace>DotNetty.Codecs</Namespace>
  <Namespace>DotNetty.Transport.Bootstrapping</Namespace>
  <Namespace>DotNetty.Transport.Channels.Sockets</Namespace>
  <Namespace>DotNetty.Handlers.Logging</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>DotNetty.Transport.Channels.Groups</Namespace>
  <Namespace>System.Net</Namespace>
</Query>

//https://github.com/Azure/DotNetty/blob/dev/examples/Factorial.Server/Program.cs

InternalLoggerFactory.DefaultFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug));

IEventLoopGroup bossGroup;
IEventLoopGroup workerGroup;

bossGroup = new MultithreadEventLoopGroup(1);
workerGroup = new MultithreadEventLoopGroup();

var STRING_ENCODER = new StringEncoder();
var STRING_DECODER = new StringDecoder();
var SERVER_HANDLER = new SecureChatServerHandler();


try
{
	var bootstrap = new ServerBootstrap();
	bootstrap.Group(bossGroup, workerGroup);

	bootstrap.Channel<TcpServerSocketChannel>();
	bootstrap
		//存放已完成三次握手的请求的等待队列的最大长度;
		.Option(ChannelOption.SoBacklog, 100)
		.Handler(new LoggingHandler(DotNetty.Handlers.Logging.LogLevel.INFO))
		.ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
		{
			IChannelPipeline pipeline = channel.Pipeline;
			pipeline.AddLast(new DelimiterBasedFrameDecoder(8192, Delimiters.LineDelimiter()));
			pipeline.AddLast(STRING_ENCODER, STRING_DECODER, SERVER_HANDLER);
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


public class SecureChatServerHandler : SimpleChannelInboundHandler<string>
{
	static volatile IChannelGroup group;

	public override void ChannelActive(IChannelHandlerContext contex)
	{
		IChannelGroup g = group;
		if (g == null)
		{
			lock (this)
			{
				if (group == null)
				{
					g = group = new DefaultChannelGroup(contex.Executor);
				}
			}
		}

		contex.WriteAndFlushAsync(string.Format("Welcome to {0} secure chat server!\n", Dns.GetHostName()));
		g.Add(contex.Channel);
	}

	class EveryOneBut : IChannelMatcher
	{
		readonly IChannelId id;

		public EveryOneBut(IChannelId id)
		{
			this.id = id;
		}

		public bool Matches(IChannel channel) => channel.Id != this.id;
	}

	protected override void ChannelRead0(IChannelHandlerContext contex, string msg)
	{
		//send message to all but this one
		string broadcast = string.Format("[{0}] {1}\n", contex.Channel.RemoteAddress, msg);
		string response = string.Format("[you] {0}\n", msg);
		group.WriteAndFlushAsync(broadcast, new EveryOneBut(contex.Channel.Id));
		contex.WriteAndFlushAsync(response);

		if (string.Equals("bye", msg, StringComparison.OrdinalIgnoreCase))
		{
			contex.CloseAsync();
		}
	}

	public override void ChannelReadComplete(IChannelHandlerContext ctx) => ctx.Flush();

	public override void ExceptionCaught(IChannelHandlerContext ctx, Exception e)
	{
		Console.WriteLine("{0}", e.StackTrace);
		ctx.CloseAsync();
	}

	public override bool IsSharable => true;
}
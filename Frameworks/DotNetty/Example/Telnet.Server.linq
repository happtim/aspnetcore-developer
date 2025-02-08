<Query Kind="Statements">
  <NuGetReference Version="0.7.5">DotNetty.Handlers</NuGetReference>
  <NuGetReference Version="0.7.5">DotNetty.Transport</NuGetReference>
  <NuGetReference Version="6.0.0">Microsoft.Extensions.Logging.Console</NuGetReference>
  <Namespace>DotNetty.Common.Internal.Logging</Namespace>
  <Namespace>DotNetty.Transport.Channels</Namespace>
  <Namespace>DotNetty.Transport.Bootstrapping</Namespace>
  <Namespace>DotNetty.Transport.Channels.Sockets</Namespace>
  <Namespace>DotNetty.Handlers.Logging</Namespace>
  <Namespace>DotNetty.Codecs</Namespace>
  <Namespace>Microsoft.Extensions.Logging</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
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
var SERVER_HANDLER = new TelnetServerHandler();


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


public class TelnetServerHandler : SimpleChannelInboundHandler<string>
{
	public override void ChannelActive(IChannelHandlerContext contex)
	{
		contex.WriteAsync(string.Format("Welcome to {0} !\r\n", Dns.GetHostName()));
		contex.WriteAndFlushAsync(string.Format("It is {0} now !\r\n", DateTime.Now));
	}

	protected override void ChannelRead0(IChannelHandlerContext contex, string msg)
	{
		// Generate and write a response.
		string response;
		bool close = false;
		if (string.IsNullOrEmpty(msg))
		{
			response = "Please type something.\r\n";
		}
		else if (string.Equals("bye", msg, StringComparison.OrdinalIgnoreCase))
		{
			response = "Have a good day!\r\n";
			close = true;
		}
		else
		{
			response = "Did you say '" + msg + "'?\r\n";
		}

		Task wait_close = contex.WriteAndFlushAsync(response);
		if (close)
		{
			Task.WaitAll(wait_close);
			contex.CloseAsync();
		}
	}

	public override void ChannelReadComplete(IChannelHandlerContext contex)
	{
		contex.Flush();
	}

	public override void ExceptionCaught(IChannelHandlerContext contex, Exception e)
	{
		Console.WriteLine("{0}", e.StackTrace);
		contex.CloseAsync();
	}

	public override bool IsSharable => true;
}
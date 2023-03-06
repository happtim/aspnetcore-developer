<Query Kind="Statements">
  <NuGetReference>DotNetty.Handlers</NuGetReference>
  <NuGetReference>DotNetty.Transport</NuGetReference>
  <NuGetReference Version="6.0.0">Microsoft.Extensions.Logging.Console</NuGetReference>
  <Namespace>DotNetty.Common.Internal.Logging</Namespace>
  <Namespace>Microsoft.Extensions.Logging</Namespace>
  <Namespace>DotNetty.Transport.Channels</Namespace>
  <Namespace>DotNetty.Transport.Bootstrapping</Namespace>
  <Namespace>DotNetty.Transport.Channels.Sockets</Namespace>
  <Namespace>DotNetty.Codecs</Namespace>
  <Namespace>System.Net</Namespace>
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

			pipeline.AddLast(new DelimiterBasedFrameDecoder(8192, Delimiters.LineDelimiter()));
			pipeline.AddLast(new StringEncoder(), new StringDecoder(), new SecureChatClientHandler());
		}));

	IChannel bootstrapChannel = await bootstrap.ConnectAsync(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8007));

	for (;;)
	{
	    string line = Console.ReadLine();
	    if (string.IsNullOrEmpty(line))
	    {
	        continue;
	    }

	    try
	    {
	        await bootstrapChannel.WriteAndFlushAsync(line + "\r\n");
		}
		catch
		{}
		if (string.Equals(line, "bye", StringComparison.OrdinalIgnoreCase))
		{
			await bootstrapChannel.CloseAsync();
			break;
		}
	}

	await bootstrapChannel.CloseAsync();
}
finally
{
	await group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
}

 public class SecureChatClientHandler : SimpleChannelInboundHandler<string>
    {
        protected override void ChannelRead0(IChannelHandlerContext contex, string msg) => Console.WriteLine(msg);

        public override void ExceptionCaught(IChannelHandlerContext contex, Exception e)
        {
            Console.WriteLine(DateTime.Now.Millisecond);
            Console.WriteLine(e.StackTrace);
            contex.CloseAsync();
        }
    }
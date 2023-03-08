<Query Kind="Statements">
  <NuGetReference>DotNetty.Codecs.Http</NuGetReference>
  <NuGetReference>DotNetty.Handlers</NuGetReference>
  <NuGetReference>DotNetty.Transport</NuGetReference>
  <NuGetReference Version="6.0.0">Microsoft.Extensions.Logging.Console</NuGetReference>
  <Namespace>DotNetty.Buffers</Namespace>
  <Namespace>DotNetty.Common.Internal.Logging</Namespace>
  <Namespace>DotNetty.Handlers.Logging</Namespace>
  <Namespace>DotNetty.Transport.Bootstrapping</Namespace>
  <Namespace>DotNetty.Transport.Channels</Namespace>
  <Namespace>DotNetty.Transport.Channels.Sockets</Namespace>
  <Namespace>Microsoft.Extensions.Logging</Namespace>
  <Namespace>System.Threading.Channels</Namespace>
  <Namespace>DotNetty.Codecs.Http</Namespace>
  <Namespace>DotNetty.Common</Namespace>
  <Namespace>DotNetty.Common.Utilities</Namespace>
  <Namespace>System.Net</Namespace>
</Query>

//https://github.com/Azure/DotNetty/blob/dev/examples/HttpServer/Program.cs

//http://localhost:8007/plaintext
//http://localhost:8007/json

InternalLoggerFactory.DefaultFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug));

IEventLoopGroup group;
IEventLoopGroup workGroup;

group = new MultithreadEventLoopGroup(1);
workGroup = new MultithreadEventLoopGroup();

try
{
	var bootstrap = new ServerBootstrap();
	bootstrap
		.Group(group, workGroup)
		.Channel<TcpServerSocketChannel>()
		.Option(ChannelOption.SoBacklog, 8192)
		.ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
		{
			IChannelPipeline pipeline = channel.Pipeline;
			pipeline.AddLast("encoder", new HttpResponseEncoder());
			pipeline.AddLast("decoder", new HttpRequestDecoder(4096, 8192, 8192, false));
			pipeline.AddLast("handler", new HelloServerHandler());
		}));

	IChannel boundChannel = await bootstrap.BindAsync(IPAddress.Any,8007); 	
	Console.WriteLine($"Httpd started. Listening on {boundChannel.LocalAddress}");
	Console.ReadLine();

	await boundChannel.CloseAsync();
}
finally
{
	await group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
}

sealed class HelloServerHandler : ChannelHandlerAdapter
{
	static readonly ThreadLocalCache Cache = new ThreadLocalCache();

	sealed class ThreadLocalCache : FastThreadLocal<AsciiString>
	{
		protected override AsciiString GetInitialValue()
		{
			DateTime dateTime = DateTime.UtcNow;
			return AsciiString.Cached($"{dateTime.DayOfWeek}, {dateTime:dd MMM yyyy HH:mm:ss z}");
		}
	}

	static readonly byte[] StaticPlaintext = Encoding.UTF8.GetBytes("Hello, World!");
	static readonly int StaticPlaintextLen = StaticPlaintext.Length;
	static readonly IByteBuffer PlaintextContentBuffer = Unpooled.UnreleasableBuffer(Unpooled.DirectBuffer().WriteBytes(StaticPlaintext));
	static readonly AsciiString PlaintextClheaderValue = AsciiString.Cached($"{StaticPlaintextLen}");
	static readonly AsciiString JsonClheaderValue = AsciiString.Cached($"{JsonLen()}");

	static readonly AsciiString TypePlain = AsciiString.Cached("text/plain");
	static readonly AsciiString TypeJson = AsciiString.Cached("application/json");
	static readonly AsciiString ServerName = AsciiString.Cached("Netty");
	static readonly AsciiString ContentTypeEntity = HttpHeaderNames.ContentType;
	static readonly AsciiString DateEntity = HttpHeaderNames.Date;
	static readonly AsciiString ContentLengthEntity = HttpHeaderNames.ContentLength;
	static readonly AsciiString ServerEntity = HttpHeaderNames.Server;

	volatile ICharSequence date = Cache.Value;

	static int JsonLen() => Encoding.UTF8.GetBytes(NewMessage().ToJsonFormat()).Length;

	static MessageBody NewMessage() => new MessageBody("Hello, World!");

	public override void ChannelRead(IChannelHandlerContext ctx, object message)
	{
		if (message is IHttpRequest request)
		{
			try
			{
				this.Process(ctx, request);
			}
			finally
			{
				ReferenceCountUtil.Release(message);
			}
		}
		else
		{
			ctx.FireChannelRead(message);
		}
	}

	void Process(IChannelHandlerContext ctx, IHttpRequest request)
	{
		string uri = request.Uri;
		switch (uri)
		{
			case "/plaintext":
				this.WriteResponse(ctx, PlaintextContentBuffer.Duplicate(), TypePlain, PlaintextClheaderValue);
				break;
			case "/json":
				byte[] json = Encoding.UTF8.GetBytes(NewMessage().ToJsonFormat());
				this.WriteResponse(ctx, Unpooled.WrappedBuffer(json), TypeJson, JsonClheaderValue);
				break;
			default:
				var response = new DefaultFullHttpResponse(DotNetty.Codecs.Http.HttpVersion.Http11, HttpResponseStatus.NotFound, Unpooled.Empty, false);
				ctx.WriteAndFlushAsync(response);
				ctx.CloseAsync();
				break;
		}
	}

	void WriteResponse(IChannelHandlerContext ctx, IByteBuffer buf, ICharSequence contentType, ICharSequence contentLength)
	{
		// Build the response object.
		var response = new DefaultFullHttpResponse(DotNetty.Codecs.Http.HttpVersion.Http11, HttpResponseStatus.OK, buf, false);
		HttpHeaders headers = response.Headers;
		headers.Set(ContentTypeEntity, contentType);
		headers.Set(ServerEntity, ServerName);
		headers.Set(DateEntity, this.date);
		headers.Set(ContentLengthEntity, contentLength);

		// Close the non-keep-alive connection after the write operation is done.
		ctx.WriteAsync(response);
	}

	public override void ExceptionCaught(IChannelHandlerContext context, Exception exception) => context.CloseAsync();

	public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();
}

sealed class MessageBody
{
	public MessageBody(string message)
	{
		this.Message = message;
	}

	public string Message { get; }

	public string ToJsonFormat() => "{" + $"\"{nameof(MessageBody)}\" :" + "{" + $"\"{nameof(this.Message)}\"" + " :\"" + this.Message + "\"}" + "}";
}
<Query Kind="Statements">
  <NuGetReference>IdentityModel.AspNetCore</NuGetReference>
  <NuGetReference Version="6.0.1">Microsoft.Extensions.Configuration.UserSecrets</NuGetReference>
  <NuGetReference>Microsoft.Extensions.DependencyInjection</NuGetReference>
  <NuGetReference>Microsoft.Extensions.Http</NuGetReference>
  <NuGetReference>Serilog.Extensions.Logging</NuGetReference>
  <NuGetReference>Serilog.Sinks.Console</NuGetReference>
  <Namespace>IdentityModel.AspNetCore.AccessTokenManagement</Namespace>
  <Namespace>IdentityModel.Client</Namespace>
  <Namespace>Microsoft.Extensions.Configuration</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Microsoft.Extensions.Logging</Namespace>
  <Namespace>Serilog</Namespace>
  <Namespace>Serilog.Extensions.Logging</Namespace>
  <Namespace>Serilog.Sinks.SystemConsole.Themes</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Net.Http.Json</Namespace>
  <Namespace>System.Text.Json.Serialization</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Web</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>

// Create a new instance of the service collection
var service = new ServiceCollection();

var configuration = new ConfigurationBuilder()
	.AddUserSecrets("linqpad-9013DB32-D1F9-400A-9057-4CFF2B283D2D")
	.Build();

var client_id = configuration["BaiduAPIKey"];
var client_secret = configuration["BaiduSecretKey"];

Log.Logger = new LoggerConfiguration()
		.MinimumLevel.Debug()
		.WriteTo.Console(theme: AnsiConsoleTheme.Code)
		.CreateLogger();

service.AddSingleton<ILoggerFactory>(_ => new SerilogLoggerFactory());

service.AddAccessTokenManagement(options =>
{
	options.Client.Clients.Add("wenxinworkshop", new ClientCredentialsTokenRequest
	{
		Address = $"https://aip.baidubce.com/oauth/2.0/token?client_id={client_id}&client_secret={ client_secret}",
	});
});

service
	.AddHttpClient("wenxinworkshop", configureClient: client =>
	{
		client.BaseAddress = new Uri("https://aip.baidubce.com/rpc/2.0/ai_custom/");
	})
	.AddHttpMessageHandler(provider =>
	{
		var accessTokenManagementService = provider.GetRequiredService<IClientAccessTokenManagementService>();

		return new WenxinWorkshopAccessTokenHandler(accessTokenManagementService, "wenxinworkshop");
	});


service.AddClientAccessTokenHttpClient("wenxinworkshop", "wenxinworkshop", configureClient: client =>
{
	client.BaseAddress = new Uri("https://aip.baidubce.com/rpc/2.0/ai_custom/");
});

var serviceProvider = service.BuildServiceProvider();
var loggerProvider = serviceProvider.GetService<ILoggerProvider>();

// Get an instance of my service
var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
var httpClient =  httpClientFactory.CreateClient("wenxinworkshop");

var response = await httpClient.PostAsJsonAsync("v1/wenxinworkshop/chat/completions", 
	new ChatRequest { 
		Messages = new List<ChatRequestMessage >() { 
			new ChatRequestMessage  {Role = ChatRole.ChatRoleUser , Content = "介绍一下你自己"}}
		});

var reply = await response.Content.ReadAsStringAsync();
reply.Dump();


public class ChatRole
{
	public static string ChatRoleUser { get; set; } = "user";
	public static string ChatRoleAssistant { get; set; } = "assistant";
}

public class ChatRequest 
{	
	[JsonPropertyName("messages")]
	public List<ChatRequestMessage > Messages { get; set;} = new List<ChatRequestMessage>();
	[JsonPropertyName("temperature")]
	public float? Temperature {get;set;} = 0.95f;
	[JsonPropertyName("top_p")]
	public float? TopP {get;set;} = 0.8f;
	[JsonPropertyName("penalty_score")]
	public float? PenaltyScore {get;set;} =1.0f;
	[JsonPropertyName("stream")]
	public bool? Stream {get;set;} = false;
	[JsonPropertyName("user_id")]
	public string UserId {get;set;}
}

public class ChatRequestMessage 
{
	[JsonPropertyName("role")]
	public string Role { get; set; } //当前支持以下： user: 表示用户 assistant: 表示对话助手
	[JsonPropertyName("content")]
	public string Content {get;set;} //对话内容，不能为空
}

public class ChatResponse 
{
	[JsonPropertyName("id")]
	public string ID { get; set; } //本轮对话的id
	[JsonPropertyName("object")]
	public string Object { get; set; } //回包类型 chat.completion：多轮对话返回
	[JsonPropertyName("created")]
	public int Created {get;set;} //时间戳
	[JsonPropertyName("sentence_id")]
	public int SentenceID {get;set;} //表示当前子句的序号。只有在流式接口模式下会返回该字段
	[JsonPropertyName("is_end")]
	public bool IsEnd {get;set; } //表示当前子句是否是最后一句。只有在流式接口模式下会返回该字段
	[JsonPropertyName("is_truncated")]
	public bool IsTruncated {get;set;}//当前生成的结果是否被截断
	[JsonPropertyName("result")]
	public string Result { get; set; } //对话返回结果
	[JsonPropertyName("need_clear_history")]
	public bool NeedClearHistory { get; set; } //表示用户输入是否存在安全，是否关闭当前会话，清理历史回话信息
	[JsonPropertyName("usage")]
	public ChatResponseUsage Usage {get;set;} //token统计信息，token数 = 汉字数+单词数*1.3 （仅为估算逻辑）
}

public class ChatResponseUsage 
{
	[JsonPropertyName("prompt_tokens")]
	public int PromptTokens {get;set;} //问题tokens数
	[JsonPropertyName("completion_tokens")]
	public int CompletionTokens {get;set;} //回答tokens数
	[JsonPropertyName("total_tokens")]
	public int TotalTokens {get;set;} //tokens总数
}

public class WenxinWorkshopAccessTokenHandler : ClientAccessTokenHandler 
{
	private readonly IClientAccessTokenManagementService _accessTokenManagementService;
	private readonly string _tokenClientName;
	
	public WenxinWorkshopAccessTokenHandler(
		IClientAccessTokenManagementService accessTokenManagementService,
		string tokenClientName
	):base(accessTokenManagementService,tokenClientName){
		
		_accessTokenManagementService = accessTokenManagementService;
		_tokenClientName = tokenClientName;
	}
	protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		return base.Send(request, cancellationToken);
	}

	protected override async Task SetTokenAsync(HttpRequestMessage request, bool forceRenewal, CancellationToken cancellationToken)
	{
		var parameters = new ClientAccessTokenParameters
		{
			ForceRenewal = forceRenewal
		};
		var token = await _accessTokenManagementService.GetClientAccessTokenAsync(_tokenClientName, parameters, cancellationToken);

		if (!string.IsNullOrWhiteSpace(token))
		{
			// 创建请求的 Uri 对象，并通过构造函数添加查询参数
			UriBuilder uriBuilder = new UriBuilder(request.RequestUri);
			var query = HttpUtility.ParseQueryString(uriBuilder.Query);

			// 追加新的查询参数
			query.Add("access_token", token);

			// 设置查询参数
			uriBuilder.Query = query.ToString();
			
			request.RequestUri = uriBuilder.Uri;
		}

	}
}
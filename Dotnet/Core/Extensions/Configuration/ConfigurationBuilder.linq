<Query Kind="Statements">
  <NuGetReference>Microsoft.Extensions.Configuration</NuGetReference>
  <NuGetReference>Microsoft.Extensions.Configuration.UserSecrets</NuGetReference>
  <Namespace>Microsoft.Extensions.Configuration</Namespace>
</Query>


//ConfigurationBuilder：
//	ConfigurationBuilder 是配置系统的入口点，用于构建和组织配置树。
//	它可以配置一个或多个 ConfigurationSource，并将它们连接在一起以加载和提供配置信息。
//	ConfigurationBuilder 提供了一种简单的方式来组织和管理不同来源的配置。
//
//ConfigurationSource：
//	ConfigurationSource 代表一个特定的配置源，例如文件、环境变量、命令行参数等。
//	每个 ConfigurationSource 是一个独立的实例，负责从特定来源加载配置数据。它可以是内置的，也可以根据需要自定义。
//	常见的 ConfigurationSource 包括 FileConfigurationSource（从文件加载配置）、EnvironmentVariablesConfigurationSource（从环境变量加载配置）等。
//
//ConfigurationProvider：
//	ConfigurationProvider 是真正负责加载和提供配置数据的组件。每个 ConfigurationSource 都对应一个 ConfigurationProvider，
//	它负责将源中的配置数据转换为 IConfiguration 实例，并提供对这些数据的访问。
//	ConfigurationProvider 还可以监听配置变更，并在配置发生变化时触发相应的事件。
//
//综上所述，这三个组件之间的关系如下：
//ConfigurationBuilder 包含一个或多个 ConfigurationSource，并使用这些 source 创建和配置一个或多个 ConfigurationProvider。
//ConfigurationProvider 则负责从每个与其关联的 ConfigurationSource 加载配置数据，并合并为一个完整的 IConfiguration 对象。
//也就是说，ConfigurationBuilder 使用 ConfigurationSource 来创建和配置 ConfigurationProvider，从而建立起配置树。

// 创建 ConfigurationBuilder 实例
var builder = new ConfigurationBuilder()
	.AddInMemoryCollection(
	new[] {
		new KeyValuePair<string, string>("Jwt:Secret", "F-JaNdRfUserjd89#5*6Xn2r5usErw8x/A?D(G+KbPeShV"),
		new KeyValuePair<string, string>("Jwt:Issuer", "http://localhost:5000/"),
		new KeyValuePair<string, string>("Jwt:Audience", "http://localhost:5000/"),
		new KeyValuePair<string, string>("Jwt:AccessTokenExpiration", "5"),
		new KeyValuePair<string, string>("Jwt:RefreshTokenExpiration", "10"),
	})
	//.AddUserSecrets("linqpad-9013DB32-D1F9-400A-9057-4CFF2B283D2D")
	.SetBasePath(AppContext.BaseDirectory) // 设置配置文件的基路径
	.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true); // 添加用于加载配置的 JSON 文件

// 构建 IConfiguration 实例
IConfiguration configuration = builder.Build();

// 从配置中读取值
var mySetting = configuration["MySetting"];
Console.WriteLine($"MySetting: {mySetting}");
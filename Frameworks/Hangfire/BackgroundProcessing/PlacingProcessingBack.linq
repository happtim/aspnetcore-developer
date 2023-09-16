<Query Kind="Statements">
  <NuGetReference Version="1.8.5">Hangfire</NuGetReference>
  <NuGetReference>Hangfire.Redis.StackExchange</NuGetReference>
  <Namespace>StackExchange.Redis</Namespace>
  <Namespace>Hangfire</Namespace>
  <Namespace>Hangfire.Redis.StackExchange</Namespace>
</Query>

// 创建ConfigurationOptions对象并设置连接配置
ConfigurationOptions configOptions = new ConfigurationOptions
{
	EndPoints = { "127.0.0.1:6379" }, // 设置Redis服务器的主机名和端口号
	DefaultDatabase = 0 // 设置默认的数据库索引号
};

GlobalConfiguration.Configuration
			   .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
			   .UseColouredConsoleLogProvider()
			   .UseSimpleAssemblyNameTypeSerializer()
			   .UseRecommendedSerializerSettings()
			   .UseRedisStorage(ConnectionMultiplexer.Connect(configOptions));

using (var server = new BackgroundJobServer())
{
	Console.ReadLine();
}
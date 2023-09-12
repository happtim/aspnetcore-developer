<Query Kind="Statements">
  <NuGetReference>Microsoft.Extensions.Hosting</NuGetReference>
  <NuGetReference>Microsoft.Extensions.Localization</NuGetReference>
  <Namespace>Microsoft.Extensions.Hosting</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Microsoft.Extensions.Localization</Namespace>
  <Namespace>System.Diagnostics.CodeAnalysis</Namespace>
  <Namespace>Microsoft.Extensions.Logging</Namespace>
  <Namespace>System.Globalization</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection.Extensions</Namespace>
  <Namespace>Microsoft.Extensions.Options</Namespace>
  <Namespace>System.Text.Json</Namespace>
</Query>


CultureInfo.CurrentCulture =
	   CultureInfo.CurrentUICulture =
		   CultureInfo.GetCultureInfo("zh-Hans");

HostApplicationBuilder builder = Host.CreateApplicationBuilder();

builder.Services.AddLocalization(options => {options.ResourcesPath = "Resources"; });
builder.Services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>((serviceProvider) => 
{
	var resourcesPath = Path.GetDirectoryName( Util.CurrentQueryPath);
	var options =  serviceProvider.GetRequiredService<IOptions<LocalizationOptions>>();
	return new JsonStringLocalizerFactory( Path.Combine( resourcesPath,options.Value.ResourcesPath));
});

builder.Services.AddTransient<MessageService>();
builder.Services.AddTransient<ParameterizedMessageService>();
builder.Logging.SetMinimumLevel(LogLevel.Warning);

using IHost host = builder.Build();

IServiceProvider services = host.Services;

ILogger logger =
	services.GetRequiredService<ILoggerFactory>()
		.CreateLogger("");

MessageService messageService =services.GetRequiredService<MessageService>();
logger.LogWarning("{Msg}",	messageService.GetGreetingMessage());

ParameterizedMessageService parameterizedMessageService =services.GetRequiredService<ParameterizedMessageService>();
logger.LogWarning("{Msg}",parameterizedMessageService.GetFormattedMessage(	DateTime.Today.AddDays(-3), 37.63));

await host.RunAsync();

public sealed class MessageService
{
	private readonly IStringLocalizer<MessageService> _localizer = null!;

	public MessageService(IStringLocalizer<MessageService> localizer) =>
		_localizer = localizer;

	[return: NotNullIfNotNull(nameof(_localizer))]
	public string? GetGreetingMessage()
	{
		LocalizedString localizedString = _localizer["GreetingMessage"];
		return localizedString;
	}
}

public class ParameterizedMessageService
{
	private readonly IStringLocalizer _localizer = null!;

	public ParameterizedMessageService(IStringLocalizerFactory factory) =>
		_localizer = factory.Create(typeof(ParameterizedMessageService));

	[return: NotNullIfNotNull(nameof(_localizer))]
	public string? GetFormattedMessage(DateTime dateTime, double dinnerPrice)
	{
		LocalizedString localizedString = _localizer["DinnerPriceFormat", dateTime, dinnerPrice];
		return localizedString;
	}
}

public class JsonStringLocalizer : IStringLocalizer
{
	private readonly Dictionary<string, string> _stringResources;

	public JsonStringLocalizer(Type resourceSource, string resourcesPath)
	{
		var resourceFileName = $"{CultureInfo.CurrentUICulture.Name}.json";
		var resourceFilePath = Path.Combine(resourcesPath, resourceFileName);

		var jsonString = File.ReadAllText(resourceFilePath);
		_stringResources = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString);
	}

	public JsonStringLocalizer(string baseName, string resourcesPath)
	{
		var resourceFilePath = Path.Combine(resourcesPath, $"{baseName}.json");

		var jsonString = File.ReadAllText(resourceFilePath);
		_stringResources = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString);
	}

	public LocalizedString this[string name]
	{
		get
		{
			if (_stringResources.TryGetValue(name, out var value))
			{
				return new LocalizedString(name, value);
			}

			return new LocalizedString(name, name, true);
		}
	}


	public LocalizedString this[string name, params object[] arguments]
	{
		get
		{
			if (_stringResources.TryGetValue(name, out var value))
			{
				return new LocalizedString(name, string.Format(value, arguments));
			}

			return new LocalizedString(name, string.Format(name, arguments), true);
		}
	}


	public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
	{
		return _stringResources.Select(r => new LocalizedString(r.Key, r.Value));
	}


	public IStringLocalizer WithCulture(CultureInfo culture)
	{
		return this;
	}
}

public class JsonStringLocalizerFactory : IStringLocalizerFactory
{
	private readonly string _resourcesPath;

	public JsonStringLocalizerFactory(string resourcesPath)
	{
		_resourcesPath = resourcesPath;
	}

	public IStringLocalizer Create(Type resourceSource)
	{
		return new JsonStringLocalizer(resourceSource, _resourcesPath);
	}

	public IStringLocalizer Create(string baseName, string location)
	{
		return new JsonStringLocalizer(baseName, _resourcesPath);
	}
}
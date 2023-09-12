using System.Globalization;
using System.Threading.Tasks;
using Main.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Volo.Abp.DependencyInjection;

namespace Main;

public class HelloWorldService : ITransientDependency
{
    public ILogger<HelloWorldService> Logger { get; set; }
    private readonly IStringLocalizer<TestResource> _stringLocalizer;
    private readonly IStringLocalizer<Subordinate.Localization.TestResource> _stringLocalizer2;

    public HelloWorldService(
        IStringLocalizer<TestResource> stringLocalizer,
        IStringLocalizer<Subordinate.Localization.TestResource> stringLocalizer2)
    {
        _stringLocalizer = stringLocalizer;
        _stringLocalizer2 = stringLocalizer2;
        Logger = NullLogger<HelloWorldService>.Instance;
    }

    public Task SayHelloAsync()
    {
        CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en");
        Logger.LogInformation(_stringLocalizer["M:HelloWorld"]);
        Logger.LogInformation(_stringLocalizer2["S:HelloWorld"]);
        CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("zh-Hans");
        Logger.LogInformation(_stringLocalizer["M:HelloWorld"]);
        Logger.LogInformation(_stringLocalizer2["S:HelloWorld"]);
        Logger.LogInformation(_stringLocalizer["ThisFieldIsInvalid."]);
        return Task.CompletedTask;
    }
}

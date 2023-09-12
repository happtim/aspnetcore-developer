using Subordinate.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace Subordinate;

[DependsOn(
    typeof(AbpLocalizationModule))]
public class SubordinateModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            // "YourRootNameSpace" is the root namespace of your project. It can be empty if your root namespace is empty.
            options.FileSets.AddEmbedded<SubordinateModule>();
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            //Define a new localization resource (TestResource)
            options.Resources
                .Add<TestResource>("en")
                .AddVirtualJson("/Subordinate/Localization/Test");
        });
    }
}
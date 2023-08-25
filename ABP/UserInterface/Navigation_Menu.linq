<Query Kind="Statements">
  <NuGetReference Version="6.0.3">Volo.Abp.Autofac</NuGetReference>
  <NuGetReference Version="6.0.3">Volo.Abp.UI.Navigation</NuGetReference>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Microsoft.Extensions.Options</Namespace>
  <Namespace>System.Security.Claims</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Volo.Abp</Namespace>
  <Namespace>Volo.Abp.Authorization</Namespace>
  <Namespace>Volo.Abp.Authorization.Permissions</Namespace>
  <Namespace>Volo.Abp.Autofac</Namespace>
  <Namespace>Volo.Abp.Modularity</Namespace>
  <Namespace>Volo.Abp.Security.Claims</Namespace>
  <Namespace>Volo.Abp.UI.Navigation</Namespace>
  <Namespace>Volo.Abp.DependencyInjection</Namespace>
</Query>

// 1: Create the ABP application container
using var application = await AbpApplicationFactory.CreateAsync<AbpUiNavigationTestModule>(options => 
{
	options.UseAutofac();
});

// 2: Initialize/start the ABP Framework (and all the modules)
await application.InitializeAsync();

Console.WriteLine("ABP Framework has been started...");

// 3: Stop the ABP Framework (and all the modules)
await application.ShutdownAsync();


[DependsOn(typeof(AbpUiNavigationModule))]
[DependsOn(typeof(AbpAuthorizationModule))]
[DependsOn(typeof(AbpAutofacModule))]
public class AbpUiNavigationTestModule : AbpModule
{
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		Configure<AbpNavigationOptions>(options =>
		{
			options.MenuContributors.Add(new TestMenuContributor1());
			options.MenuContributors.Add(new TestMenuContributor2());
		});
	}

	public override async Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
	{
		var currentPrincipalAccessor = context.ServiceProvider.GetRequiredService<ICurrentPrincipalAccessor>();
		
		var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
		{
			new Claim(ClaimTypes.Name,"bob"),
			new Claim(ClaimTypes.NameIdentifier,"123456")
		}));
		
		using(currentPrincipalAccessor.Change(claimsPrincipal))
		{
			var menuManager = context.ServiceProvider.GetRequiredService<IMenuManager>();

			var mainMenu = await menuManager.GetMainMenuAsync();

			mainMenu.Dump();
		}

	}
}

/* Adds menu items:
 * - Administration
 *   - User Management
 *   - Role Management
 */
public class TestMenuContributor1 : IMenuContributor
{
	public Task ConfigureMenuAsync(MenuConfigurationContext context)
	{
		if (context.Menu.Name != StandardMenus.Main)
		{
			return Task.CompletedTask;
		}

		context.Menu.DisplayName = "Main Menu1";

		var administration = context.Menu.GetAdministration();

		administration.AddItem(new ApplicationMenuItem("Administration.UserManagement", "User Management", url: "/admin/users").RequirePermissions("Administration.UserManagement"));
		administration.AddItem(new ApplicationMenuItem("Administration.RoleManagement", "Role Management", url: "/admin/roles").RequirePermissions("Administration.RoleManagement"));

		return Task.CompletedTask;
	}
}

/* Adds menu items:
 * - Dashboard
 * - Administration
 *   - Dashboard Settings
 */
public class TestMenuContributor2 : IMenuContributor
{
	public Task ConfigureMenuAsync(MenuConfigurationContext context)
	{
		if (context.Menu.Name != StandardMenus.Main)
		{
			return Task.CompletedTask;
		}

		context.Menu.Items.Insert(0, new ApplicationMenuItem("Dashboard", "Dashboard", url: "/dashboard").RequirePermissions("Dashboard"));

		var administration = context.Menu.GetAdministration();

		administration.AddItem(new ApplicationMenuItem("Administration.DashboardSettings", "Dashboard Settings", url: "/admin/settings/dashboard").RequirePermissions("Administration.DashboardSettings"));

		administration.AddItem(
			new ApplicationMenuItem("Administration.SubMenu1", "Sub menu 1", url: "/submenu1")
				.AddItem(new ApplicationMenuItem("Administration.SubMenu1.1", "Sub menu 1.1", url: "/submenu1/submenu1_1").RequirePermissions("Administration.SubMenu1.1"))
				.AddItem(new ApplicationMenuItem("Administration.SubMenu1.2", "Sub menu 1.2", url: "/submenu1/submenu1_2").RequirePermissions("Administration.SubMenu1.2"))
		);

		return Task.CompletedTask;
	}
}

public class FakePermissionStore : IPermissionStore, ITransientDependency
{
	public Task<bool> IsGrantedAsync(string name, string providerName, string providerKey)
	{
		var result = (name.Contains("Administration") || name.Contains("Dashboard")) && !name.Contains("SubMenu1");
		return Task.FromResult(result);
	}

	public Task<MultiplePermissionGrantResult> IsGrantedAsync(string[] names, string providerName, string providerKey)
	{
		var result = new MultiplePermissionGrantResult();
		foreach (var name in names)
		{
			result.Result.Add(name, (name.Contains("Administration") || name.Contains("Dashboard")) && !name.Contains("SubMenu1")
				? PermissionGrantResult.Granted
				: PermissionGrantResult.Prohibited);
		}

		return Task.FromResult(result);
	}
}

public class TestPermissionDefinitionProvider : PermissionDefinitionProvider
{
	public override void Define(IPermissionDefinitionContext context)
	{
		var group = context.AddGroup("TestGroup");

		group.AddPermission("Dashboard");

		group.AddPermission("Administration");
		group.AddPermission("Administration.UserManagement");
		group.AddPermission("Administration.RoleManagement");

		group.AddPermission("Administration.DashboardSettings");

		group.AddPermission("Administration.SubMenu1");
		group.AddPermission("Administration.SubMenu1.1");
		group.AddPermission("Administration.SubMenu1.2");
	}
}



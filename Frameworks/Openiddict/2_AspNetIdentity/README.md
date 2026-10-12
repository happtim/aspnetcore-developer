# openiddict结合ASP.NET Core Identity

openiddict原本使用数据存储clients，claims等数据。但是没有存储用户信息的数据，这节我们将为上一节的案例增加asp.net core identity。

## 修改项目

### 配置服务和dbcontext

```
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet add package Microsoft.AspNetCore.Identity.UI
```

程序启动开始注册服务，添加AddIdentity。并且在Data目录创建ApplicationUser

```
// Register the Identity services.  
services.AddIdentity<ApplicationUser, IdentityRole>()  
	.AddEntityFrameworkStores<ApplicationDbContext>()  
	.AddDefaultTokenProviders()  
	.AddDefaultUI();
```

修改ApplicationDbContext继承关系，引入了Identity之后要继承IdentityDbContext\<ApplicationUser\>。

```
public class ApplicationDbContext: IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
```

### Adding Migrations

要创建迁移，您需要在您的计算机上安装Entity Framework Core CLI工具

```
dotnet tool install --global dotnet-ef
dotnet add package Microsoft.EntityFrameworkCore.Design
```

现在，请在src/OpeniddictServer目录下运行以下两个命令以创建迁移：

```
dotnet ef migrations add Initial -o Data/Migrations/
```

### 应用迁移

我们在SeedData类中进行数据创建和初始化，前一节我们直接将数据删除后重新初始化，这节我们将数据初始化方式改成，先删除，再数据迁移，再初始化数据。

```
var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();  
await context.Database.EnsureDeletedAsync();  
await context.Database.MigrateAsync();
```

### 初始化用户

我们使用bob和alice两个用户分别初始化用户信息。

### 修改使用用户名登录

默认IdentityUI使用email登入。 我们初始化数据的时候没有使用email作为用户。需要修改login界面。

```
/// <summary>
///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
///     directly from your code. This API may change or be removed in future releases.
/// </summary>
public class InputModel
{
	/// <summary>
	///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
	///     directly from your code. This API may change or be removed in future releases.
	/// </summary>
	[Required]
	public string Username { get; set; }

	/// <summary>
	///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
	///     directly from your code. This API may change or be removed in future releases.
	/// </summary>
	[Required]
	[DataType(DataType.Password)]
	public string Password { get; set; }

	/// <summary>
	///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
	///     directly from your code. This API may change or be removed in future releases.
	/// </summary>
	[Display(Name = "Remember me?")]
	public bool RememberMe { get; set; }
}
```
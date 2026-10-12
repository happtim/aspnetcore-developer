# ASP.NET Core identity TokenProvider 理解

## 什么是Token？为什么我们需要它？

令牌是应用程序或服务可以发放给用户的一种东西，用户可以稍后将其作为证明其身份和常常是执行某个操作的授权的方式进行提交。我们可以在需要确认有关用户身份的某些信息的各种地方使用令牌，例如电话号码或电子邮件地址是否真的属于他们。它们还可以以其他方式使用；例如，Slack使用令牌在移动设备上提供魔术的登录链接。

由于这些潜在用途，确保它们的安全性和可信任性非常重要，因为如果使用不当，它们可能会在应用程序中造成安全漏洞。应该建立机制以使旧的或已使用的令牌失效，以防止其他人在获得访问权限后使用它们。ASP.NET Core Identity 通过令牌提供程序提供一些基本的令牌，以用于常见任务。默认的ASP.NET Web应用程序MVC模板在AccountController和ManageController上的一些帐户和用户管理任务中使用这些令牌。

## TokenProvider

为了获得令牌或验证令牌，我们使用令牌提供者。ASP.NET Core Identity定义了一个`IUserTwoFactorTokenProvider`接口，任何令牌提供者都应该实现该接口。该接口非常简单，定义了三个方法：

```csharp
/// <summary>  
/// Provides an abstraction for two-factor token generators.  
/// </summary>  
/// <typeparam name="TUser">The type encapsulating a user.</typeparam>  
public interface IUserTwoFactorTokenProvider<TUser> where TUser : class  
{  
/// <summary>  
/// Generates a token for the specified <paramref name="user" /> and <paramref name="purpose" />.  
/// </summary>  
Task<string> GenerateAsync(string purpose, UserManager<TUser> manager, TUser user);  
  
/// <summary>  
/// Returns a flag indicating whether the specified <paramref name="token" /> is valid for the given  
/// <paramref name="user" /> and <paramref name="purpose" />.  
/// </summary>  
Task<bool> ValidateAsync(string purpose, string token, UserManager<TUser> manager, TUser user);  
  
/// <summary>  
/// Returns a flag indicating whether the token provider can generate a token suitable for two-factor authentication token for  
/// the specified <paramref name="user" />.  
/// </summary>  
Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<TUser> manager, TUser user);  
}
```

- GenerateAsync方法用于生成一个用于指定用户（user）和用途（purpose）的令牌（token），返回一个包含生成的令牌的字符串。
- ValidateAsync方法用于验证给定用户（user）和用途（purpose）下的指定令牌（token）是否有效，返回一个bool类型的标志指示令牌是否有效。
- CanGenerateTwoFactorTokenAsync方法用于判断令牌提供程序是否能够为给定用户（user）生成适用于两步验证的令牌，返回一个bool类型的标志指示是否能够生成令牌。


您可以将任意数量的令牌提供者注册到您的项目中，以满足您的需求。默认情况下，IdentityBuilder具有一个方法AddDefaultTokenProviders()，您可以在项目的启动文件中的AddIdentity调用中链接使用。这将按照下面的代码注册3个默认提供者。令牌提供者需要在DI容器中注册，以便在需要时进行注入。

```csharp
public static IdentityBuilder AddDefaultTokenProviders(this IdentityBuilder builder)  
{  
	var userType = builder.UserType;  
	var dataProtectionProviderType = typeof(DataProtectorTokenProvider<>).MakeGenericType(userType);  
	var phoneNumberProviderType = typeof(PhoneNumberTokenProvider<>).MakeGenericType(userType);  
	var emailTokenProviderType = typeof(EmailTokenProvider<>).MakeGenericType(userType);  
	var authenticatorProviderType = typeof(AuthenticatorTokenProvider<>).MakeGenericType(userType);  
	return builder.AddTokenProvider(TokenOptions.DefaultProvider, dataProtectionProviderType)  
	.AddTokenProvider(TokenOptions.DefaultEmailProvider, emailTokenProviderType)  
	.AddTokenProvider(TokenOptions.DefaultPhoneProvider, phoneNumberProviderType)  
	.AddTokenProvider(TokenOptions.DefaultAuthenticatorProvider, authenticatorProviderType);  
}
```

这段代码使用了TokenOptions类，该类定义了一些常用的提供者名称，并维护了一个可用提供者的字典，其中键是提供者名称，值是被注册的提供者的类型。AddTokenProvider的代码如下所示。

```csharp
public virtual IdentityBuilder AddTokenProvider(string providerName, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type provider)
{
	if (!typeof(IUserTwoFactorTokenProvider<>).MakeGenericType(UserType).IsAssignableFrom(provider))
	{
		throw new InvalidOperationException(Resources.FormatInvalidManagerType(provider.Name, "IUserTwoFactorTokenProvider", UserType.Name));
	}
	Services.Configure<IdentityOptions>(options =>
	{
		options.Tokens.ProviderMap[providerName] = new TokenProviderDescriptor(provider);
	});
	Services.AddTransient(provider);
	return this;
}
```

## 邮件确认

现在我已经介绍了什么是令牌以及它们是如何注册的，我认为最好的做法是看一下生成和验证令牌的过程。我选择了逐步介绍创建电子邮件确认令牌的过程。用户收到一个包含用户ID和令牌作为查询字符串参数的链接，该链接指向“ConfirmEmail”操作。用户需要点击链接，然后令牌将被验证，并将其电子邮件标记为已确认。

以这种方式验证电子邮件是一种良好的做法，因为它可以防止人们使用或添加不属于他们自己的邮箱进行注册。通过向电子邮件地址发送一个链接，并要求用户在激活电子邮件之前执行某个操作，只有邮箱的真正所有者才能访问该链接并点击以确认他们确实注册了该账户。我们相信用户的操作，基于我们为他们提供的安全措施。由于令牌经过加密保护，它们不容易被伪造。

### 生成Token

ASP.NET Core Identity提供了生成要发给用户的令牌所需的类。将身份系统用于请求令牌并在链接中包含令牌的实际操作由MVC网站本身进行管理，并根据需要调用身份API。

在ASP.NET MVC项目中，确认电子邮件的生成是可选的，并且默认情况下未启用。但是，代码位于AccountController中，但已被注释掉。Identity中的UserManager类提供了调用生成令牌并在以后验证令牌所需的所有方法。一旦从Identity库中获取了令牌，我们就可以在发送激活电子邮件时使用该令牌。

在我们的示例中，我们可以调用GenerateEmailConfirmationTokenAsync(TUser user)方法。我们将要生成令牌的用户传递给该方法。

```csharp
public virtual Task<string> GenerateEmailConfirmationTokenAsync(TUser user)  
{  
	ThrowIfDisposed();  
	return GenerateUserTokenAsync(user, Options.Tokens.EmailConfirmationTokenProvider, ConfirmEmailTokenPurpose);  
}
```

GenerateUserTokenAsync方法需要用户、要使用的令牌提供程序的名称（从身份选项中提取）和作为字符串的令牌用途。 ConfirmEmailTokenPurpose是一个常量字符串，定义了要使用的措辞。在这种情况下，它是"EmailConfirmation"。

每个令牌都应该有一个目的，这样它们可以与系统中的特定操作紧密关联。一个令牌只能用于一个操作，不能在其他操作中使用。

```csharp
public virtual Task<string> GenerateUserTokenAsync(TUser user, string tokenProvider, string purpose)
{
	ThrowIfDisposed();
	if (user == null)
	{
		throw new ArgumentNullException(nameof(user));
	}
	if (tokenProvider == null)
	{
		throw new ArgumentNullException(nameof(tokenProvider));
	}
	if (!_tokenProviders.ContainsKey(tokenProvider))
	{
		throw new NotSupportedException(Resources.FormatNoTokenProvider(nameof(TUser), tokenProvider));
	}

	return _tokenProviders[tokenProvider].GenerateAsync(purpose, this, user);
}
```

在通常的空检查之后，这实际上是通过检查基于传递给该方法的tokenProvider参数的UserManager可用的token提供程序字典来进行检查。一旦找到提供程序，就会调用其GenerateAsync方法。

现在我们来看一下DataProtectionTokenProvider上的GenerateAsync方法。

```csharp
public virtual async Task<string> GenerateAsync(string purpose, UserManager<TUser> manager, TUser user)
{
	ArgumentNullException.ThrowIfNull(user);
	var ms = new MemoryStream();
	var userId = await manager.GetUserIdAsync(user);
	using (var writer = ms.CreateWriter())
	{
		writer.Write(DateTimeOffset.UtcNow);
		writer.Write(userId);
		writer.Write(purpose ?? "");
		string? stamp = null;
		if (manager.SupportsUserSecurityStamp)
		{
			stamp = await manager.GetSecurityStampAsync(user);
		}
		writer.Write(stamp ?? "");
	}
	var protectedBytes = Protector.Protect(ms.ToArray());
	return Convert.ToBase64String(protectedBytes);
}
```

该方法使用内存流来构建一个包含以下元素的字节数组：

1. 当前的UTC时间
2. user id
4. purpose 如果有的话
5. 用户安全标识如果当前UserManager支持。安全标识是一个在数据库中与用户关联的GUID。当在Identity UserManager类中执行某些操作时，它会被更新，并且可以用于在账户更改时使旧的令牌失效。例如，当我们更改用户的用户名或电子邮件地址时，安全标识会被更改，在令牌中将安全标识更改，以防止再次使用相同的令牌确认电子邮件，因为令牌中的安全标识将不再与用户的当前安全标识匹配。
  
然后，这些元素会传递给通过注入的IDataProtector上的Protect方法。对于本文，详细解释数据保护器可能会有点复杂，也会偏离主题。我计划在将来更深入地研究它们，但目前可以说数据保护库定义了用于保护数据的加密API。Identity使用此API从其令牌提供程序中获取加密和解密令牌。

一旦返回受保护的字节，它们将被进行base64编码后返回。

### 验证Token

一旦用户在确认电子邮件中点击链接，它将把他们带到 AccountController 中的 ConfirmEmail 动作。该动作接收链接中的用户ID和代码（受保护的令牌）。然后该动作将调用 UserManager 的 ConfirmEmailAsync 方法，该方法将进一步调用 VerifyUserTokenAsync 方法。该方法将从 ProviderMap 获取适当的令牌提供程序，并调用 ValidateAsync 方法。

```csharp
public virtual async Task<bool> ValidateAsync(string purpose, string token, UserManager<TUser> manager, TUser user)
{
	try
	{
		var unprotectedData = Protector.Unprotect(Convert.FromBase64String(token));
		var ms = new MemoryStream(unprotectedData);
		using (var reader = ms.CreateReader())
		{
			var creationTime = reader.ReadDateTimeOffset();
			var expirationTime = creationTime + Options.TokenLifespan;
			if (expirationTime < DateTimeOffset.UtcNow)
			{
				Logger.InvalidExpirationTime();
				return false;
			}

			var userId = reader.ReadString();
			var actualUserId = await manager.GetUserIdAsync(user);
			if (userId != actualUserId)
			{
				Logger.UserIdsNotEquals();
				return false;
			}

			var purp = reader.ReadString();
			if (!string.Equals(purp, purpose))
			{
				Logger.PurposeNotEquals(purpose, purp);
				return false;
			}

			var stamp = reader.ReadString();
			if (reader.PeekChar() != -1)
			{
				Logger.UnexpectedEndOfInput();
				return false;
			}

			if (manager.SupportsUserSecurityStamp)
			{
				var isEqualsSecurityStamp =  q == await manager.GetSecurityStampAsync(user);
				if (!isEqualsSecurityStamp)
				{
					Logger.SecurityStampNotEquals();
				}

				return isEqualsSecurityStamp;
			}

			var stampIsEmpty = stamp == "";
			if (!stampIsEmpty)
			{
				Logger.SecurityStampIsNotEmpty();
			}

			return stampIsEmpty;
		}
	}
	// ReSharper disable once EmptyGeneralCatchClause
	catch
	{
		// Do not leak exception
		Logger.UnhandledException();
	}

	return false;
}
```

令牌首先从base64字符串表示转换为字节数组。然后将其传递给IDataProtector进行解密。再次说明，这个过程的详细细节对于本帖来说太复杂了。解密后的内容被传递到新的内存流中进行读取。

首先从令牌的开头读取创建时间。到期时间是通过获取令牌的创建时间并添加在DataProtectionTokenProviderOptions中定义的令牌生命周期来计算的。默认情况下，此设置为1天。如果令牌已过期，则方法返回false，因为它不再被视为有效令牌。

然后读取userId字符串并将其与用户的id进行比较（这是根据他们在电子邮件中收到的链接中的userId来进行的。账户控制器首先使用该id从数据库中加载用户。这确保令牌属于尝试使用它的用户。

接下来读取目的并检查它是否与正在进行验证的目的相匹配（这将通过调用者传递给该方法）。这确保令牌只对特定功能有效。

然后读取安全标记并将其存储在一个本地变量中，以便在稍后使用。

然后调用PeekChar，它尝试从令牌中获取（但不移动）下一个字符。由于我们应该已经到达流的末尾，它检查-1是否表示没有更多字符可用。任何其他值都表示该令牌有额外的数据，因此无效。

最后，如果当前用户管理器支持安全标记，则从用户存储中检索用户的安全标记，并将其与从令牌中读取的标记进行比较。假设它们匹配，那么我们现在可以确认该令牌确实有效，并将该响应返回给调用者。
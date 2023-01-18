<Query Kind="Statements">
  <NuGetReference>Castle.Core</NuGetReference>
  <Namespace>Castle.DynamicProxy</Namespace>
</Query>

//https://kozmic.net/2009/04/27/castle-dynamic-proxy-tutorial-part-x-interface-proxies-with-target/


//这种代理是其他两种接口代理类型的混合体。它允许，但不要求提供目标对象。
//它还允许在代理的生存期内交换目标。它不绑定到一种类型的代理目标，因此只要它们实现目标接口，
//就可以将一种代理类型重用于不同的目标类型。

//当我说大多数用户永远不会使用它时，我的意思是大约 99%。我个人没有用过。
//这种代理类型在极少数情况下使用，您可以随意跳过阅读此部分，因为您可能不需要此处提供的信息。

//正如您可能从名称中推断的那样，它与带有目标的接口代理非常相似。
//它在一个重要方面基本上是不同的：它允许您将调用目标交换为目标接口的不同实现。

var _primaryStorage = new PrimaryStorage { IsUp = true };
var _sut = new StorageFactory(_primaryStorage);
var _secondaryStorage = new SecondaryStorage();
_sut.SecondaryStorage = _secondaryStorage;

IStorage storage = _sut.GetStorage();
string message1 = "message1";
string message2 = "message2";
string message3 = "message3";

storage.Save(message1);
_primaryStorage.IsUp = false;
storage.Save(message2);
_primaryStorage.IsUp = true;
storage.Save(message3);
IList<object> primary = _primaryStorage.Items;
IList<object> secondary = _secondaryStorage.Items;

primary.Dump();
secondary.Dump();


public class StorageInterceptor : IInterceptor
{
	private readonly IStorage _secondaryStorage;

	public StorageInterceptor(IStorage secondaryStorage)
	{
		_secondaryStorage = secondaryStorage;
	}

	public void Intercept(IInvocation invocation)
	{
		var primaryStorage = invocation.InvocationTarget as PrimaryStorage;
		if (primaryStorage.IsUp == false)
		{
			ChangeToSecondaryStorage(invocation);
		}
		invocation.Proceed();
	}

	private void ChangeToSecondaryStorage(IInvocation invocation)
	{
		var changeProxyTarget = invocation as IChangeProxyTarget;
		changeProxyTarget.ChangeInvocationTarget(_secondaryStorage);
	}
}

public class StorageFactory
{
	private readonly IStorage _primaryStorage;
	private ProxyGenerator _generator;

	public StorageFactory(IStorage primaryStorage)
	{
		_primaryStorage = primaryStorage;
		_generator = new ProxyGenerator();
	}

	public IStorage SecondaryStorage { private get; set; }

	public IStorage GetStorage()
	{
		var interceptor = new StorageInterceptor(SecondaryStorage);
		object storage = _generator.CreateInterfaceProxyWithTargetInterface(typeof(IStorage), _primaryStorage, interceptor);
		return storage as IStorage;
	}
}

public class PrimaryStorage : IStorage
{
	private IList<object> _items = new List<object>();

	public IList<object> Items
	{
		get { return _items; }
	}

	public bool IsUp { get; set; }

	public void Save(object data)
	{
		_items.Add(data);
	}
}

public class SecondaryStorage : IStorage
{
	private IList<object> _items = new List<object>();

	public IList<object> Items
	{
		get { return _items; }
	}

	public void Save(object data)
	{
		_items.Add(data);
	}
}

public interface IStorage
{
	void Save(object data);
}
<Query Kind="Statements">
  <NuGetReference>Castle.Core</NuGetReference>
  <Namespace>Castle.DynamicProxy</Namespace>
</Query>

//https://kozmic.net/2009/08/12/castle-dynamic-proxy-tutorial-part-xiii-mix-in-this-mix/

//到目前为止，我们已经涵盖了动态代理的大部分基本功能，除了一个 - mixins。
//不涉及理论细节，mixin是一个将许多其他物体缝合在一起的对象，表现出所有这些对象的行为。
//让我用伪代码来说明这一点：


//在大多数语言中，这是通过多重继承实现的，但在 CLR 中不允许这样做，
//这对混合在动态代理中的实现和工作方式施加了某些限制。我们不能有多个基类，
//但我们可以实现多个接口，这就是动态代理用于 mixins 的接口。

//var dog = Dog.New();
//var cat = Cat.New();
//var mixin = mixin(cat, dog);
//mixin.Bark();
//mixin.Meow();


var generator = new ProxyGenerator();
var options = new ProxyGenerationOptions();
options.AddMixinInstance(new Dictionary<string, object>());

var person =  generator.CreateClassProxy<Person>(options);

var dictionary = (IDictionary)person;
dictionary.Add("Next Leave", DateTime.Now.AddMonths(4));

UseSomewhereElse(person);

static void UseSomewhereElse(Person person)
{
	var dictionary = (IDictionary<string, object>)person;
	var date = ((DateTime)dictionary["Next Leave"]).Date;
	Console.WriteLine($"Next leave date of {person.Name} is {date}");
}

public class Person
{
	public string Name { get; set; }
	public int Age { get; set; }
}
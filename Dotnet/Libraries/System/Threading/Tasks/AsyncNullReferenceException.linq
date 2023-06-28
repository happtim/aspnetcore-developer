<Query Kind="Statements">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>


Person p = new Person(){Name = "123"} ;

//通过异步的方式获取name
var name =  await p.GetNameAsync();
name.Dump();

p = null;

//可空的方式获取Name
name = p?.GetName();
name.Dump();

if (p != null)
{
	name = await p?.GetNameAsync();
	name.Dump();
}

//异步可空的方式获取Name
name =  await p?.GetNameAsync();
name.Dump();


public class Person{
	public string Name {get;set;}
	
	public async Task<string> GetNameAsync(){
		await Task.CompletedTask;
		return Name;
	}
	
	public string GetName(){
		return Name;
	}
}
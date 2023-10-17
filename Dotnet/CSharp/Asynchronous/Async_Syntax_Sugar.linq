<Query Kind="Statements">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>


var result = await ConvertAsync();
	result = await ConvertAsync2();
	result = await ConvertAsync3();

//如何不加async 会报错
Task<string> ConvertAsync3()
{
	return null;
}

//加了async 不会报错
async Task<string> ConvertAsync()
{
	return null;
}

//不加async 但是返回Task也不会报错
Task<string> ConvertAsync2()
{
	return Task.FromResult<string>(null);
}



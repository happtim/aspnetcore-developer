<Query Kind="Statements">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>


var result = await ConvertAsync();

//如何不加async 会报错
async Task<string> ConvertAsync()
{
	return null;
}

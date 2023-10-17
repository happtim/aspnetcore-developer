<Query Kind="Statements">
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Net.Http.Json</Namespace>
</Query>


//请求的url地址一般为通常返回Json格式数据， System.Net.Http.Json 提供了扩展方法自动执行序列化和反序列化。

// 根据httpclient使用指南，建议在应用程序的生命周期内重复使用 HttpClient 实例。
HttpClient sharedClient = new()
{
	BaseAddress = new Uri("https://jsonplaceholder.typicode.com"),
};

await GetFromJsonAsync(sharedClient);

await PostAsJsonAsync(sharedClient);


async Task GetFromJsonAsync(HttpClient httpClient)
{
	var todos = await httpClient.GetFromJsonAsync<List<Todo>>(
		"todos?userId=1&completed=false");

	Console.WriteLine("GET https://jsonplaceholder.typicode.com/todos?userId=1&completed=false HTTP/1.1");
	
	todos.Dump();
}

async Task PostAsJsonAsync(HttpClient httpClient)
{
	using HttpResponseMessage response = await httpClient.PostAsJsonAsync(
		"todos",
		new Todo(UserId: 9, Id: 99, Title: "Show extensions", Completed: false));
		
	Console.WriteLine("POST https://jsonplaceholder.typicode.com/todos HTTP/1.1");

	response.EnsureSuccessStatusCode();

	var todo = await response.Content.ReadFromJsonAsync<Todo>();
	todo.Dump();
}


public record class Todo(
	int? UserId = null,
	int? Id = null,
	string? Title = null,
	bool? Completed = null);

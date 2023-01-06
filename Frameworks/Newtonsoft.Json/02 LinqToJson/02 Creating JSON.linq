<Query Kind="Statements">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
</Query>

// https://www.newtonsoft.com/json/help/html/CreatingLINQtoJSON.htm

//除了从现有 JSON 字符串解析 JSON 之外，还可以从头开始创建 LINQ to JSON 对象以创建新的 JSON 结构。

//Manually Creating JSON
JArray array = new JArray();
JValue text = new JValue("Manual text");
JValue date = new JValue(new DateTime(2000, 5, 23));

array.Add(text);
array.Add(date);
array.Dump();

//Creating JSON with LINQ
//使用 LINQ 以声明方式创建 JSON 对象是从值集合创建 JSON 的方法。
List<Post> posts = new List<Post>()
	{
		new Post()
		{
			Title = "文章",
			Description = "描述",
			Link = "http://example.com",
			Categories = new List<string>(){"new","health"}
		},
		new Post()
		{
			Title = "文章2",
			Description = "描述2",
			Link = "http://example.com",
			Categories = new List<string>(){"new","health"}
		},
	};

JObject rss =
	new JObject(
		new JProperty("channel",
			new JObject(
				new JProperty("title", "James Newton-King"),
				new JProperty("link", "http://james.newtonking.com"),
				new JProperty("description", "James Newton-King's blog."),
				new JProperty("item",
					new JArray(
						from p in posts
						orderby p.Title
						select new JObject(
							new JProperty("title", p.Title),
							new JProperty("description", p.Description),
							new JProperty("link", p.Link),
							new JProperty("category",
								new JArray(
									from c in p.Categories
									select new JValue(c)))))))));
rss.ToString().Dump();

//Creating JSON from an object
//最后一个选项是使用 FromObject() 方法从非 JSON 类型创建 JSON 对象。
//在内部，FromObject 将使用 JsonSerializer 将对象序列化为 LINQ to JSON 对象，而不是文本。
JObject o = JObject.FromObject(new
{
	channel = new
	{
		title = "James Newton-King",
		link = "http://james.newtonking.com",
		description = "James Newton-King's blog.",
		item =
			from p in posts
			orderby p.Title
			select new
			{
				title = p.Title,
				description = p.Description,
				link = p.Link,
				category = p.Categories
			}
	}
});

o.Dump();

public class Post
{
	public string Title {get;set;}
	public string Description {get;set;}
	public string Link {get;set;}
	public List<string> Categories{get;set;}
}
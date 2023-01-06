<Query Kind="Statements">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
</Query>

//https://www.newtonsoft.com/json/help/html/QueryingLINQtoJSON.htm

//LINQ to JSON 提供了许多从其对象获取数据的方法。
//JObject/JArray 上的索引方法允许您通过集合中的对象或索引的属性名称快速获取数据，
//而 Children() 允许您获取 IEnumerable 形式的数据范围<JToken>，然后使用 LINQ 进行查询。

// Getting values by Property Name or Collection Index
string json = @"{
  'channel': {
    'title': 'James Newton-King',
    'link': 'http://james.newtonking.com',
    'description': 'James Newton-King\'s blog.',
    'item': [
      {
        'title': 'Json.NET 1.3 + New license + Now on CodePlex',
        'description': 'Announcing the release of Json.NET 1.3, the MIT license and the source on CodePlex',
        'link': 'http://james.newtonking.com/projects/json-net.aspx',
        'categories': [
          'Json.NET',
          'CodePlex'
        ]
      },
      {
        'title': 'LINQ to JSON beta',
        'description': 'Announcing LINQ to JSON',
        'link': 'http://james.newtonking.com/projects/json-net.aspx',
        'categories': [
          'Json.NET',
          'LINQ'
        ]
      }
    ]
  }
}";

JObject rss = JObject.Parse(json);

string rssTitle = (string)rss["channel"]["title"];
rssTitle.Dump();

string itemTitle = (string)rss["channel"]["item"][0]["title"];
itemTitle.Dump();

JArray categories = (JArray)rss["channel"]["item"][0]["categories"];
categories.Dump();

IList<string> categoriesText = categories.Select(c => (string)c).ToList();
categoriesText.Dump();


//Querying with LINQ;

//JObject/JArray 也可以使用 LINQ 进行查询。Children() 将 JObject/JArray 的子值作为 IEnumerable 返回<JToken>，
//然后可以使用标准的 Where/OrderBy/Select LINQ 运算符进行查询。

var postTitles =
	from p in rss["channel"]["item"]
	select (string)p["title"];

foreach (var item in postTitles)
{
	item.Dump();
}

var categories2 =
	from c in rss["channel"]["item"].SelectMany(i => i["categories"]).Values<string>()
	group c by c
	into g
	orderby g.Count() descending
	select new { Category = g.Key, Count = g.Count() };

foreach (var c in categories2)
{
	Console.WriteLine(c.Category + " - Count: " + c.Count);
}
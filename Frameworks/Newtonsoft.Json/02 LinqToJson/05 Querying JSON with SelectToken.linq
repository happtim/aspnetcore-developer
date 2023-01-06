<Query Kind="Statements">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
</Query>

//https://www.newtonsoft.com/json/help/html/SelectToken.htm

//SelectToken() 提供了一种使用单个字符串路径查询 LINQ to JSON 的方法，该路径指向所需的 JToken。
//SelectToken 使动态查询变得容易，因为整个查询是在字符串中定义的。


//SelectToken

//SelectToken 是 JToken 上的一种方法，它采用子令牌的字符串路径。
//如果在路径位置找不到令牌，则 SelectToken 返回子令牌或 null 引用。

JObject o = JObject.Parse(@"{
  'Stores': [
    'Lambton Quay',
    'Willis Street'
  ],
  'Manufacturers': [
    {
      'Name': 'Acme Co',
      'Products': [
        {
          'Name': 'Anvil',
          'Price': 50
        }
      ]
    },
    {
      'Name': 'Contoso',
      'Products': [
        {
          'Name': 'Elbow Grease',
          'Price': 99.95
        },
        {
          'Name': 'Headlight Fluid',
          'Price': 4
        }
      ]
    }
  ]
}");

string name = (string)o.SelectToken("Manufacturers[0].Name");
name.Dump();

decimal productPrice = (decimal)o.SelectToken("Manufacturers[0].Products[0].Price");
productPrice.Dump();

string productName = (string)o.SelectToken("Manufacturers[1].Products[0].Name");
productName.Dump();


//SelectToken with JSONPath

// SelectToken 支持 JSONPath 查询。在此处了解有关 JSONPath 的更多信息。 https://goessner.net/articles/JsonPath/

// manufacturer with the name 'Acme Co'
JToken acme = o.SelectToken("$.Manufacturers[?(@.Name == 'Acme Co')]");
acme.Dump();

// name of all products priced 50 and above
IEnumerable<JToken> pricyProducts = o.SelectTokens("$..Products[?(@.Price >= 50)].Name");

foreach (JToken item in pricyProducts)
{
	Console.WriteLine(item);
}


//SelectToken with LINQ

IList<string> storeNames = o.SelectToken("Stores").Select(s => (string)s).ToList();
storeNames.Dump();

IList<string> firstProductNames = o["Manufacturers"].Select(m => (string)m.SelectToken("Products[1].Name")).ToList();
firstProductNames.Dump();

decimal totalPrice = o["Manufacturers"].Sum(m => (decimal)m.SelectToken("Products[0].Price"));
totalPrice.Dump();

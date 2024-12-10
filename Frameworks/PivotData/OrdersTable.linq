<Query Kind="Statements" />

static DataTable GetOrdersTable()
{
	// sample "orders" table that contains 1,000 rows
	var t = new DataTable("orders");
	t.Columns.Add("product", typeof(string));
	t.Columns.Add("country", typeof(string));
	t.Columns.Add("city", typeof(string)); 
	t.Columns.Add("quantity", typeof(int));
	t.Columns.Add("total", typeof(decimal));
  	t.Columns.Add("orderDate", typeof(DateTime));

	var countries = new[] { "USA", "United Kingdom", "Germany", "Italy", "France", "Canada", "Spain" };
	var cities = new Dictionary<string, string[]>
	{
		["USA"] = new[] { "New York", "Los Angeles", "Chicago", "Houston" },
		["United Kingdom"] = new[] { "London", "Manchester", "Birmingham", "Liverpool" },
		["Germany"] = new[] { "Berlin", "Munich", "Hamburg", "Frankfurt" },
		["Italy"] = new[] { "Rome", "Milan", "Naples", "Turin" },
		["France"] = new[] { "Paris", "Marseille", "Lyon", "Toulouse" },
		["Canada"] = new[] { "Toronto", "Vancouver", "Montreal", "Calgary" },
		["Spain"] = new[] { "Madrid", "Barcelona", "Valencia", "Seville" }
	};
	var products = new[] { "Product #1", "Product #2", "Product #3" };
	var productPrices = new decimal[] { 21, 33, 78 };
	

	for (int i = 1; i <= 1000; i++)
	{
		var q = 1 + (i % 6);
		var productIdx = (i + i % 10) % products.Length;
		var country = countries[i % countries.Length];
		var cityArray = cities[country];
		var city = cityArray[i % cityArray.Length];

		// 构建日期  
		var year = 2010 + (i % 6);
		var month = 1 + (i % 12);
		var day = 1 + (i % 28); // 改为1-28避免无效日期  

		var orderDate = new DateTime(year, month, day);

		t.Rows.Add(new object[] {
					products[productIdx],
					country,
					city,
					q,
					q*productPrices[productIdx],
					orderDate
				});
	}
	t.AcceptChanges();
	return t;

}
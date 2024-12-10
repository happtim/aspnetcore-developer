<Query Kind="Statements">
  <NuGetReference>NReco.PivotData</NuGetReference>
  <NuGetReference>NReco.PivotData.Extensions</NuGetReference>
  <Namespace>NReco.PivotData</Namespace>
</Query>

#load ".\OrdersTable"

//该包位于NReco.PivotData.Extensions内。需要授权。
//可用于通过简单的搜索查询字符串对立方体进行过滤:

// sample dataset
var ordersTable = GetOrdersTable();

var cube = new PivotData(
	new[] { "Product", "Country" ,"City", "OrderDate" },
	new CompositeAggregatorFactory(
		new SumAggregatorFactory("Quantity"),        // count is a measure #0
		new SumAggregatorFactory("Total")   // sum of amount is a measure #1 
  ));
  
cube.ProcessData(new DataTableReader(ordersTable));

var cubeFilter = new CubeKeywordFilter("2015, Product #1");
var filteredPvtData = cubeFilter.Filter(cube);

filteredPvtData.Dump();
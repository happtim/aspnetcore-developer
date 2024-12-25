<Query Kind="Statements">
  <NuGetReference>NReco.PivotData</NuGetReference>
  <Namespace>NReco.PivotData</Namespace>
</Query>

#load ".\OrdersTable"

//https://blog.infodiagram.com/2019/11/olap-data-cube-presentation-powerpoint.html
//https://galaktika-soft.com/blog/overview-of-olap-technology.html

//Slice
//切片操作通过为其维度选择一个值，从立方体中提取一个子立方体。例如，
//我们可以通过选择“USA”作为国家维度的值，从立方体中检索显示所有国家所有年份的互联网销售量的“切片”。

//Dice
//切块是切片操作的“扩展”，因为它允许用户通过选择多个维度的值来提取子立方体。
//使用上面的示例，我们可以通过选择“USA”作为国家维度和“2015”作为日期维度来执行切块操作。

// sample dataset
var ordersTable = GetOrdersTable();

var cube = new PivotData(
	new[] { "Product", "Country" , "OrderDate" },
	new CompositeAggregatorFactory(
		new CountAggregatorFactory(),        // count is a measure #0
		new SumAggregatorFactory("Total")   // sum of amount is a measure #1 
  ));
  
cube.ProcessData(new DataTableReader(ordersTable));

//1. 筛选cube
var whereQuery = new SliceQuery(cube)
	.Where(
		 "Product",  // dimension to filter 
		 "Product #1", "Product #2"  // several values for IN match
	)
	.Where(
		 "OrderDate",
		 (dimKey) => ((DateTime)dimKey).Year == 2015
	);
var slicedPvtData = whereQuery.Execute();  // resulted cube is filtered by product and year

// 获取所有产品在 USA 的汇总值  
var usaAggr = slicedPvtData[Key.Empty, "USA" , Key.Empty];
usaAggr.AsComposite().Aggregators[0].Value.Dump("Product #1 and #2 in 2015 USA Count:");
usaAggr.AsComposite().Aggregators[1].Value.Dump("Product #1 and #2 in 2015 Total Sum:");

slicedPvtData.Dump();


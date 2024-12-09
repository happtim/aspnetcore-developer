<Query Kind="Statements">
  <NuGetReference>NReco.PivotData</NuGetReference>
  <Namespace>NReco.PivotData</Namespace>
</Query>

#load ".\OrdersTable"

//卷起(Roll Up)：涉及沿着一个维度对数据进行总结。

// sample dataset
var ordersTable = GetOrdersTable();

var cube = new PivotData(
	new[] { "Product", "Country" ,"City" },
	new CompositeAggregatorFactory(
		new CountAggregatorFactory(),        // count is a measure #0
		new SumAggregatorFactory("Total")   // sum of amount is a measure #1 
  ));
  
cube.ProcessData(new DataTableReader(ordersTable));

//3. 指定维度 (roll-up)
var selectQuery3 = new SliceQuery(cube).Dimension("Product").Dimension("City");
var slicedPvtData3 = selectQuery3.Execute(); // resulted cube has only "Product" and "City" dimensions

slicedPvtData3.Dump();



<Query Kind="Statements">
  <NuGetReference>NReco.PivotData</NuGetReference>
  <Namespace>NReco.PivotData</Namespace>
</Query>

#load ".\OrdersTable"

// sample dataset
var ordersTable = GetOrdersTable();

//ordersTable.Dump();

var cube = new PivotData(
	new[] { "Product", "Country" }, 
	new CompositeAggregatorFactory(
		new CountAggregatorFactory(),        // count is a measure #0
		new SumAggregatorFactory("Total")   // sum of amount is a measure #1 
  ));

cube.ProcessData(new DataTableReader(ordersTable));

//数据访问
//通过维度键的索引器访问：
IAggregator m = cube["Product #1", "USA"]; // number of keys should match cube dimensions
object mValues = m.Value;                // type of m.Value depends on the aggregate function
m.AsComposite().Aggregators[0].Value.Dump("Product1 USA Count:");  // access composite aggregator value #0
m.AsComposite().Aggregators[1].Value.Dump("Product1 USA Total Sum:");    // access composite aggregator value #1

//Key.Empty 用于按某些维度进行汇总并获取小计值：
var product1Aggr = cube["Product #1", Key.Empty];
product1Aggr.AsComposite().Aggregators[0].Value.Dump("Product1 All Countries Count:");
product1Aggr.AsComposite().Aggregators[1].Value.Dump("Product1 All Countries Total Sum:");

// 获取所有产品在 USA 的汇总值  
var usaAggr = cube[Key.Empty, "USA"];
usaAggr.AsComposite().Aggregators[0].Value.Dump("All Products USA Count:");
usaAggr.AsComposite().Aggregators[1].Value.Dump("All Products USA Total Sum:");


// 获取所有产品所有国家的总计值  
var totalAggr = cube[Key.Empty, Key.Empty];
totalAggr.AsComposite().Aggregators[0].Value.Dump("Grand Total Count:");
totalAggr.AsComposite().Aggregators[1].Value.Dump("Grand Total Sum:");

foreach (var entry in cube)
{
	// entry.Key holds array of dimension keys
	var measure = entry.Value; // implements IAggregator interface
	Console.WriteLine("D1={0} D2={1} V1={2} V2={3}", 
		entry.Key[0], 
		entry.Key[1], 
		((object[]) measure.Value)[0],
		((object[])measure.Value)[1]);
}

cube.Dump();


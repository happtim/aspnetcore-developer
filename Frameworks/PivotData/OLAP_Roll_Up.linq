<Query Kind="Statements">
  <NuGetReference>NReco.PivotData</NuGetReference>
  <Namespace>NReco.PivotData</Namespace>
</Query>

#load ".\OrdersTable"

//卷起(Roll Up)：Roll Up 是沿着立方体维度层次结构聚合数据。执行 Roll Up 时，数据将按升序层次结构进行聚合。
//钻取(Drii Down)：操作允许用户从最聚合的值向最详细的值逐级下降。例如，在日期维度中从“年”到“月”到“日”，在地理维度中从“国家”到“省份”到“城市”。

// sample dataset
var ordersTable = GetOrdersTable();

var cube = new PivotData(
	new[] { "Product", "Country" ,"City", "OrderDate" },
	new CompositeAggregatorFactory(
		new CountAggregatorFactory(),        // count is a measure #0
		new SumAggregatorFactory("Total")   // sum of amount is a measure #1 
  ));
  
cube.ProcessData(new DataTableReader(ordersTable));

//cube.Dump();

//roll-up City to Country
var selectQuery = new SliceQuery(cube).Dimension("Product").Dimension("Country");
var rollupPvtData = selectQuery.Execute(); // resulted cube has only "Product" and "Country" dimensions

//rollupPvtData.Dump();

//roll-up Data to month
//可以通过以下方式定义派生维度（=从现有维度键计算而来）
var byMonthCube = new SliceQuery(cube).Dimension("Product").Dimension("creation_date_month",
	(dimKeys) =>
	{  // array of entry dimension keys
		var creationDateValue = (DateTime)dimKeys[3]; // #3 - index of "orderDate" dimension
		return creationDateValue.Month;
	}
);

var monthPvtData = byMonthCube.Execute();

var byYearAndQuarter = new SliceQuery(cube).Dimension("creation_date_year_and_quarter",
	(dimKeys) =>
	{
		var creationDateValue = (DateTime)dimKeys[3]; // #3 - index of "orderDate" dimension
		return String.Format("{0} Q{1}",
			creationDateValue.Year, GetQuarter(creationDateValue));
	}
);

var quarterPvtData = byYearAndQuarter.Execute();

quarterPvtData.Dump();

int GetQuarter( DateTime date)
{
	return (date.Month - 1) / 3 + 1;
}
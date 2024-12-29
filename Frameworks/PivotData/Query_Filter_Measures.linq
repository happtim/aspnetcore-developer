<Query Kind="Statements">
  <NuGetReference>NReco.PivotData</NuGetReference>
  <Namespace>NReco.PivotData</Namespace>
</Query>

#load ".\OrdersTable"

//1.Select specific measures

//如果数据立方体具有多个度量（使用 CompositeAggregatorFactory 配置），则可以使用Measure方法仅选择特定度量。

// sample dataset
var ordersTable = GetOrdersTable();

var cube = new PivotData(
	new[] { "Product", "Country" ,"City", "OrderDate" },
	new CompositeAggregatorFactory(
		new SumAggregatorFactory("Quantity"),        // count is a measure #0
		new SumAggregatorFactory("Total")   // sum of amount is a measure #1 
  ));
  
cube.ProcessData(new DataTableReader(ordersTable));

var oneMeasurePvtData = new SliceQuery(cube).Measure(0).Measure(1).Execute(); // take only measure at index #0

oneMeasurePvtData.Dump();

//2. Define derived measure 

var derivedAggrQuery = new SliceQuery(cube)
	.Measure(
		new SumAggregatorFactory("delta_fld"),  // factory for new metric
		(aggr) =>
		{
			var compositeAggr = aggr.AsComposite();
			var aggr0val = Convert.ToDecimal(compositeAggr.Aggregators[0].Value);
			var aggr1val = Convert.ToDecimal(compositeAggr.Aggregators[1].Value);
			var diffVal = aggr0val - aggr1val;

			// result should correspond specified aggregator factory
			return new SumAggregator("delta_fld",
				// sum aggregator state
				new object[] {
  					aggr.Count, // elements count
					diffVal // new aggr sum value
				});
		}
	);
	
var derivedPvtData = derivedAggrQuery.Execute();

//derivedPvtData.Dump();

//3. Define formula measure

// average item price = sum of total / sum of quantity
var avgItemPriceByYearQuery = new SliceQuery(cube)
	.Dimension("Year",
		(dimKeys) => {
			var creationDateValue = (DateTime)dimKeys[3]; // #3 - index of "orderDate" dimension
			return creationDateValue.Year;
		})
	.Measure("Avg item price",
		(aggrArgs) =>
		{
			// value of first argument (from measure #1)
			var sumOfTotal = ConvertHelper.ConvertToDecimal(aggrArgs[1].Value, 0M);

			// value of second argument (from measure #0)
			var sumOfQuantity = ConvertHelper.ConvertToDecimal(aggrArgs[0].Value, 0M);

			if (sumOfQuantity == 0)
				return 0M; // prevent div by zero
			return sumOfTotal / sumOfQuantity;
		},
		new int[] { 0, 1} // indexes of measures for formula arguments
   );
   
var avgItemPriceByYearPvtData =  avgItemPriceByYearQuery.Execute();

avgItemPriceByYearPvtData.Dump();

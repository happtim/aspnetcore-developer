<Query Kind="Statements">
  <NuGetReference>NReco.PivotData</NuGetReference>
  <Namespace>NReco.PivotData</Namespace>
</Query>

#load ".\OrdersTable"

//一旦您有由实例表示的内存数据立方体,可以使用PivotTable来创建透视表
//PivotTable.RowKeys: 代表表格行键的 对象数组。
//PivotTable.ColumnKeys: 代表表格列键的 对象数组。
//PivotTable[int? row, int? col]  获取指定行 x 列交叉点处的数据透视表值。
//	您还可以使用来获取行和/或列的小计和总计值。

// sample dataset
var ordersTable = GetOrdersTable();

var cube = new PivotData(
	new[] { "Product", "Country" ,"City", "OrderDate" },
	new CompositeAggregatorFactory(
		new SumAggregatorFactory("Quantity"),        // count is a measure #0
		new SumAggregatorFactory("Total")   // sum of amount is a measure #1 
  ));
  
cube.ProcessData(new DataTableReader(ordersTable));

var pvtTbl = new PivotTable(
		new[] { "Product" },  // dimension(s) for table rows. List may be empty.
		new[] { "OrderDate" }, // dimension(s) for table columns
		cube);
		
var sb = new StringBuilder();
sb.Append("<table>");

// column labels
sb.Append("<tr>");
sb.Append("<th></th>");
foreach (var colKey in pvtTbl.ColumnKeys)
{
	sb.AppendFormat("<th>{0}</th>", colKey.ToString());
}

sb.Append("<th>Totals</th>");
sb.Append("</tr>");
for (var r = 0; r < pvtTbl.RowKeys.Length; r++) 
{
	var rowKey = pvtTbl.RowKeys[r];
	sb.Append("<tr>");
	sb.AppendFormat("<th>{0}</th>", rowKey.ToString() ); // row label
	
	for( var c = 0; c< pvtTbl.ColumnKeys.Length ; c++)
	{
		sb.AppendFormat("<td>{0}</td>", ((object[]) pvtTbl[r,c].Value)[0] );
	}
	sb.AppendFormat("<td>{0}</td>", ((object[])pvtTbl[r, null].Value)[0]); // row total
	sb.Append("</tr>");

}

// row for column totals
sb.Append("<tr>");
sb.Append("<th>Totals</th>");
for (var c = 0; c < pvtTbl.ColumnKeys.Length; c++)
{
	sb.AppendFormat("<td>{0}</td>", ((object[])pvtTbl[null, c].Value)[0]);
}
sb.AppendFormat("<td>{0}</td>", ((object[])pvtTbl[null, null].Value)[0]); // grand total
sb.Append("</tr>");

sb.Append("</table>");

Util.RawHtml(sb.ToString()).Dump();
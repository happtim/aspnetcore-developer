<Query Kind="Statements">
  <NuGetReference>NReco.PivotData</NuGetReference>
  <Namespace>NReco.PivotData</Namespace>
  <Namespace>System.ComponentModel</Namespace>
</Query>

#load ".\OrdersTable"

//默认情况下，数据透视表模型中的行和列是按标签（A-Z）排序的。

// sample dataset
var ordersTable = GetOrdersTable();

var cube = new PivotData(
	new[] { "Product", "Country" ,"City", "OrderDate" },
	new CompositeAggregatorFactory(
		new SumAggregatorFactory("Quantity"),        // count is a measure #0
		new SumAggregatorFactory("Total")   // sum of amount is a measure #1 
  ));
  
cube.ProcessData(new DataTableReader(ordersTable));

//cube.Dump();

var pvtTbl = new PivotTable(
		new[] { "Product" },  // dimension(s) for table rows. List may be empty.
		new[] { "OrderDate" }, // dimension(s) for table columns
		cube);

//索引从0开始，默认计算第一个值的排序
pvtTbl.SortRowKeys(2, ListSortDirection.Ascending);  // sort rows by column #2 values
//pvtTbl.SortColumnKeys(1, ListSortDirection.Ascending);  // sort columns by row #1 values

// apply the same order by dimension keys (labels) instead of indexes
//pvtTbl.SortRowKeysByColumnKey(new ValueKey( DateTime.Parse("2010/1/5 0:00:00")), 0 /*measureIndex*/, ListSortDirection.Ascending);
//pvtTbl.SortColumnKeysByRowKey(new ValueKey("Product #1"), 0 /*measureIndex*/, ListSortDirection.Ascending);


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
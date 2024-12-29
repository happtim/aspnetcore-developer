<Query Kind="Statements">
  <NuGetReference>NReco.PivotData</NuGetReference>
  <Namespace>NReco.PivotData</Namespace>
  <Namespace>System.ComponentModel</Namespace>
</Query>

#load ".\OrdersTable"

//数据透视表可以使用以下方法按值排序：
//1.PivotTable.SortRowKeys 按照指定列索引的值（在 PivotTable.ColumnKeys 数组中的#）对行进行排序
//2.PivotTable.SortColumnKeysByRowKey 用于按指定列键的值对行进行排序
//3.PivotTable.SortColumnKeys 按照指定行索引的值对列进行排序（在 PivotTable.RowKeys 数组中的＃）
//4.PivotTable.SortRowKeysByColumnKey 按指定行键的值对列进行排序

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
		cube,
		new CustomSortKeyComparer(new[] {  // rows comparer
    		NaturalSortKeyComparer.Instance }),
		 new CustomSortKeyComparer(new[] { // column comparer
    		NaturalSortKeyComparer.ReverseInstance }) //列Label 倒叙排序
		);
		
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
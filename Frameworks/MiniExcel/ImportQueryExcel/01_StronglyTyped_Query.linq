<Query Kind="Statements">
  <NuGetReference Version="1.35.0">MiniExcel</NuGetReference>
  <Namespace>MiniExcelLibs</Namespace>
  <Namespace>MiniExcelLibs.Attributes</Namespace>
  <Namespace>MiniExcelLibs.OpenXml</Namespace>
</Query>

// ──────────────────────────────────────────────────────────────────────────────
// Execute a query and map the results to a strongly typed IEnumerable
// Recommend to use Stream.Query because of better efficiency.
// Columns: ID | Name | BoD | Age | VIP | Mail | Points | IgnoredProperty
// ──────────────────────────────────────────────────────────────────────────────

// 1. Prepare the Excel file  "TestTypeMapping.xlsx"  with seed data
string path = Path.Combine( Path.GetDirectoryName(Util.CurrentQueryPath),"TestTypeMapping.xlsx");
path.Dump();

var configuration = new OpenXmlConfiguration
{
	DynamicColumns = new DynamicExcelColumn[]
	{
		new("ID") { Name = "Container:Id", Width = 20,Index = 0 },
		new("Name") { Name = "Container:Name", Width = 20, Index = 1 },
		new("BoD") { Name = "Container:BoD", Width = 30 ,Index = 2 }, // 即使在excel 中Bob是其他字段，也可以使用index来确保读取正确的列
	}
};

// ──────────────────────────────────────────────────────────────────────────────
// 2a. Query via static helper MiniExcel.Query<T>
// ──────────────────────────────────────────────────────────────────────────────
var rowsA = MiniExcel.Query<UserAccount>(path,configuration:configuration).ToList();
rowsA.Dump("MiniExcel.Query<UserAccount> (static)");

// ──────────────────────────────────────────────────────────────────────────────
// 2b. Query via Stream.Query<T>  ← recommended for better efficiency
// ──────────────────────────────────────────────────────────────────────────────
using (var stream = File.OpenRead(path))
{
    var rowsB = stream.Query<UserAccount>().ToList();
    rowsB.Dump("stream.Query<UserAccount> (stream – recommended)");
}

// ──────────────────────────────────────────────────────────────────────────────
// Model
// ──────────────────────────────────────────────────────────────────────────────
public class UserAccount
{
    public Guid     ID               { get; set; }
    public string   Name             { get; set; }
    public DateTime BoD              { get; set; }
    public int      Age              { get; set; }
    public bool     VIP              { get; set; }
    public string   Mail             { get; set; }
    public decimal  Points           { get; set; }

    // [ExcelIgnore] tells MiniExcel to skip this property during import/export
    [ExcelIgnore]
    public string   IgnoredProperty  { get; set; }
}

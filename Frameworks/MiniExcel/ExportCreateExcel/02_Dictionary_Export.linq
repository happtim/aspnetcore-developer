<Query Kind="Statements">
  <NuGetReference Version="1.35.0">MiniExcel</NuGetReference>
  <Namespace>MiniExcelLibs</Namespace>
  <Namespace>MiniExcelLibs.OpenXml</Namespace>
  <Namespace>MiniExcelLibs.Attributes</Namespace>
</Query>

// -----------------------------------------------------------------------------
// Export/Create Excel with IEnumerable<IDictionary<string, object>>
// Dynamic dictionary-based export — column names come from dictionary keys
// -----------------------------------------------------------------------------

var path = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "DictionaryExport.xlsx");
path.Dump("Excel output path");

// Build rows as IDictionary<string, object>
var source = new List<IDictionary<string, object>>
{
    new Dictionary<string, object>
    {
        ["OrderNo"]       = "SO-2001",
        ["CustomerName"]  = "Charlie",
        ["CustomerEmail"] = "charlie@contoso.com",
        ["Amount"]        = 350.75m,
        ["Currency"]      = "USD",
        ["PaidAt"]        = new DateTime(2026, 3, 10, 9, 0, 0)
    },
    new Dictionary<string, object>
    {
        ["OrderNo"]       = "SO-2002",
        ["CustomerName"]  = "Diana",
        ["CustomerEmail"] = "diana@contoso.com",
        ["Amount"]        = 880.00m,
        ["Currency"]      = "EUR",
        ["PaidAt"]        = new DateTime(2026, 3, 12, 15, 30, 0)
    },
    new Dictionary<string, object>
    {
        ["OrderNo"]       = "SO-2003",
        ["CustomerName"]  = "Eve",
        ["CustomerEmail"] = "eve@contoso.com",
        ["Amount"]        = 120.50m,
        ["Currency"]      = "CNY",
        ["PaidAt"]        = new DateTime(2026, 3, 13, 11, 45, 0)
    }
};

// Configure column order, header display names, widths and number formats
var configuration = new OpenXmlConfiguration
{
    DynamicColumns = new DynamicExcelColumn[]
    {
        new DynamicExcelColumn("OrderNo")       { Name = "Order No",       Index = 0, Width = 16 },
        new DynamicExcelColumn("CustomerName")  { Name = "Customer Name",  Index = 1, Width = 18 },
        new DynamicExcelColumn("CustomerEmail") { Name = "Email",          Index = 2, Width = 30 },
        new DynamicExcelColumn("Amount")        { Name = "Amount",         Index = 3, Width = 14, Format = "0.00" },
        new DynamicExcelColumn("Currency")      { Name = "Currency",       Index = 4, Width = 12 },
        new DynamicExcelColumn("PaidAt")        { Name = "Paid At",        Index = 5, Width = 24, Format = "yyyy-MM-dd HH:mm:ss" },
    },
    AutoFilter = true
};

MiniExcel.SaveAs(path, source, configuration: configuration, overwriteFile: true);
$"Saved: {path}".Dump();

// Read back as dictionary rows to verify
using (var stream = File.OpenRead(path))
{
    // Query returns ExpandoObject rows; cast to IDictionary for key-based access.
    var rows = stream.Query(useHeaderRow: true)
        .Cast<IDictionary<string, object>>()
        .ToList();
    rows.Dump("Read-back dynamic result");
}

"Dictionary export verified successfully.".Dump();

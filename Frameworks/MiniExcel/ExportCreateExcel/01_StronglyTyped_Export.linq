<Query Kind="Statements">
  <NuGetReference Version="1.35.0">MiniExcel</NuGetReference>
  <Namespace>MiniExcelLibs</Namespace>
  <Namespace>MiniExcelLibs.OpenXml</Namespace>
  <Namespace>MiniExcelLibs.Attributes</Namespace>
</Query>

// -----------------------------------------------------------------------------
// Export/Create Excel with OpenXmlConfiguration
// Simple strongly typed export (no nested properties)
// -----------------------------------------------------------------------------

var path = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "StronglyTypedExport.xlsx");
path.Dump("Excel output path");

var source = new[]
{
    new ExportOrderFlat
    {
        OrderNo = "SO-1001",
        CustomerName = "Alice",
        CustomerEmail = "alice@contoso.com",
        Amount = 199.50m,
        Currency = "USD",
        PaidAt = new DateTime(2026, 03, 10, 9, 30, 0)
    },
    new ExportOrderFlat
    {
        OrderNo = "SO-1002",
        CustomerName = "Bob",
        CustomerEmail = "bob@contoso.com",
        Amount = 520.00m,
        Currency = "CNY",
        PaidAt = new DateTime(2026, 03, 11, 14, 15, 0)
    }
};

var configuration = new OpenXmlConfiguration
{
    DynamicColumns = new DynamicExcelColumn[]
    {
        new DynamicExcelColumn("OrderNo")
        {
            Name = "OrderNo",
            Index = 0,
            Width = 16
        },
        new DynamicExcelColumn("CustomerName")
        {
            Name = "CustomerName",
            Index = 1,
            Width = 18
        },
        new DynamicExcelColumn("CustomerEmail")
        {
            Name = "CustomerEmail",
            Index = 2,
            Width = 28
        },
        new DynamicExcelColumn("Amount")
        {
            Name = "Amount",
            Index = 3,
            Width = 14,
            Format = "0.00"
        },
        new DynamicExcelColumn("Currency")
        {
            Name = "Currency",
            Index = 4,
            Width = 14
        },
        new DynamicExcelColumn("PaidAt")
        {
            Name = "PaidAt",
            Index = 5,
            Width = 24,
            Format = "yyyy-MM-dd HH:mm:ss"
        }
    },
    AutoFilter = true
};

MiniExcel.SaveAs(path, source, configuration: configuration, overwriteFile: true);

using (var stream = File.OpenRead(path))
{
    var rows = stream.Query<ExportOrderFlat>().ToList();

    rows.Dump("Read-back strongly typed result");
}

"Strongly typed export verified successfully (no nested properties).".Dump();

public class ExportOrderFlat
{
    public string OrderNo { get; set; }
    public string CustomerName { get; set; }
    public string CustomerEmail { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public DateTime PaidAt { get; set; }
}
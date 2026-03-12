<Query Kind="Statements">
  <NuGetReference Version="1.35.0">MiniExcel</NuGetReference>
  <Namespace>MiniExcelLibs</Namespace>
</Query>

// ══════════════════════════════════════════════════════════════════════════════
// PART 1 — Dynamic query WITHOUT header row
//           Dynamic keys are column letters: A, B, C, D …
// ══════════════════════════════════════════════════════════════════════════════

var pathNoHeader = Path.Combine(Path.GetTempPath(), "DynamicNoHeader.xlsx");

var rawData = new[]
{
    new { A = "MiniExcel", B = 1 },
    new { A = "Github",    B = 2 },
};

// printHeader: false → write data rows only, no header
MiniExcel.SaveAs(pathNoHeader, rawData, overwriteFile: true, printHeader: false);
$"Excel saved to: {pathNoHeader}".Dump("Part 1 – Setup");

// Stream (recommended)
using (var stream = File.OpenRead(pathNoHeader))
{
    var rowsB = stream.Query(useHeaderRow: false).ToList();
    rowsB.Dump("Part 1b – stream.Query (no header, stream – recommended)");

    Debug.Assert((string)rowsB[0].A == "MiniExcel");
    Debug.Assert((int)   rowsB[0].B == 1);
    Debug.Assert((string)rowsB[1].A == "Github");
    Debug.Assert((int)   rowsB[1].B == 2);
    "Part 1 assertions passed".Dump();
}

// ══════════════════════════════════════════════════════════════════════════════
// PART 2 — Dynamic query WITH first header row (useHeaderRow: true)
//           Dynamic keys match the header cell values: Column1, Column2 …
//           Note: if duplicate column names exist, the last (rightmost) wins.
// ══════════════════════════════════════════════════════════════════════════════

// Input Excel:
//   Column1    | Column2
//   -----------+---------
//   MiniExcel  | 1
//   Github     | 2

var pathWithHeader = Path.Combine(Path.GetTempPath(), "DynamicWithHeader.xlsx");

var headerData = new[]
{
    new { Column1 = "MiniExcel", Column2 = 1 },
    new { Column1 = "Github",    Column2 = 2 },
};

MiniExcel.SaveAs(pathWithHeader, headerData, overwriteFile: true);
$"Excel saved to: {pathWithHeader}".Dump("Part 2 – Setup");

// Stream (recommended)
using (var stream = File.OpenRead(pathWithHeader))
{
    var rows = stream.Query(useHeaderRow: true).ToList();
    rows.Dump("Part 2b – stream.Query (with header, stream – recommended)");

    Debug.Assert((string)rows[0].Column1 == "MiniExcel");
    Debug.Assert((int)   rows[0].Column2 == 1);
    Debug.Assert((string)rows[1].Column1 == "Github");
    Debug.Assert((int)   rows[1].Column2 == 2);
    "Part 2 assertions passed".Dump();
}

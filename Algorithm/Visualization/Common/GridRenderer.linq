<Query Kind="Program" />

// ============================================================
// 网格渲染器（SVG），A* 等网格寻路算法使用。
// 每个格子：底色 + 边框 + 中央大字（S/G）+ 三个角标小字（g/h/f）。
// 直接 F5 运行本文件可看渲染示例。
// ============================================================

void Main()
{
	// 渲染示例：3×4 小网格
	var cells = new GridCell[3][];
	for (int r = 0; r < 3; r++)
	{
		cells[r] = new GridCell[4];
		for (int c = 0; c < 4; c++) cells[r][c] = new GridCell();
	}
	cells[0][0].Center = "S"; cells[0][0].TextColor = "#1b5e20"; cells[0][0].Fill = "#a5d6a7";
	cells[1][1].Fill = "#455a64";                                   // 墙
	cells[0][1].Fill = "#bbdefb"; cells[0][1].TL = "g1"; cells[0][1].TR = "h3"; cells[0][1].BC = "f4";
	cells[1][0].Fill = "#ffd54f"; cells[1][0].TL = "g1"; cells[1][0].TR = "h3"; cells[1][0].BC = "f4";
	cells[2][3].Center = "G"; cells[2][3].TextColor = "#b71c1c";
	GridRenderer.Render(cells, 52, "示例：S=起点　G=终点　深灰=墙").Dump();
}

public class GridCell
{
	public string Fill { get; set; } = "#ffffff";
	public string Stroke { get; set; } = "#c0c0c0";
	public double StrokeW { get; set; } = 1;
	public string Center { get; set; }        // 中央大字（如 S / G）
	public string TextColor { get; set; } = "#333333";
	public string TL { get; set; }            // 左上角小字（如 g 值）
	public string TR { get; set; }            // 右上角小字（如 h 值）
	public string BC { get; set; }            // 底部中央小字（如 f 值）
}

public static class GridRenderer
{
	public static object Render(GridCell[][] cells, int cellSize = 52, string footer = null)
	{
		int rows = cells.Length, cols = cells[0].Length;
		int w = cols * cellSize + 2, h = rows * cellSize + 2;
		var sb = new StringBuilder();

		sb.Append("<div style='font-family:Consolas,\"Microsoft YaHei\",monospace'>");
		sb.Append($"<svg width='{w}' height='{h}' style='background:#fafafa'>");

		// 先画普通格子，再画粗框格子，避免粗框被邻格盖住
		for (int pass = 0; pass < 2; pass++)
			for (int r = 0; r < rows; r++)
				for (int c = 0; c < cols; c++)
				{
					var cell = cells[r][c];
					bool thick = cell.StrokeW > 1.01;
					if ((pass == 0) == thick) continue;

					int x = c * cellSize + 1, y = r * cellSize + 1;
					sb.Append($"<rect x='{x}' y='{y}' width='{cellSize}' height='{cellSize}' fill='{cell.Fill}' stroke='{cell.Stroke}' stroke-width='{N(cell.StrokeW)}'/>");
					if (cell.TL != null)
						sb.Append($"<text x='{x + 3}' y='{y + 12}' font-size='9' fill='#555'>{Enc(cell.TL)}</text>");
					if (cell.TR != null)
						sb.Append($"<text x='{x + cellSize - 3}' y='{y + 12}' font-size='9' text-anchor='end' fill='#555'>{Enc(cell.TR)}</text>");
					if (cell.BC != null)
						sb.Append($"<text x='{x + cellSize / 2}' y='{y + cellSize - 5}' font-size='11' font-weight='bold' text-anchor='middle' fill='#1565c0'>{Enc(cell.BC)}</text>");
					if (cell.Center != null)
						sb.Append($"<text x='{x + cellSize / 2}' y='{y + cellSize / 2 + 6}' font-size='16' font-weight='bold' text-anchor='middle' fill='{cell.TextColor}'>{Enc(cell.Center)}</text>");
				}

		sb.Append("</svg>");
		if (footer != null)
			sb.Append($"<div style='font-size:12px;color:#777;margin-top:4px;max-width:{w}px'>{Enc(footer)}</div>");
		sb.Append("</div>");
		return Util.RawHtml(sb.ToString());
	}

	static string N(double v) => v.ToString("0.##");
	static string Enc(string s) => System.Net.WebUtility.HtmlEncode(s ?? "");
}

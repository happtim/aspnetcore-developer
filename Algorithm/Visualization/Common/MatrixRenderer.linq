<Query Kind="Program" />

// ============================================================
// 成本矩阵渲染器（HTML 表格），匈牙利算法等矩阵类算法使用。
// 支持：单元格底色标记、★/′ 角标、覆盖线（红线贯穿行/列）、
//       单元格小字（如原始成本）、表格下方图例说明。
// 直接 F5 运行本文件可看渲染示例。
// ============================================================

void Main()
{
	// 渲染示例
	var vis = new MatrixVis
	{
		RowLabels = new[] { "R1", "R2", "R3" },
		ColLabels = new[] { "T1", "T2", "T3" },
		Values = new[]
		{
			new double[] { 0, 2, 5 },
			new double[] { 3, 0, 1 },
			new double[] { 4, 1, 0 },
		},
		Marks = new[]
		{
			new[] { CellMark.Star, CellMark.None, CellMark.None },
			new[] { CellMark.None, CellMark.Current, CellMark.None },
			new[] { CellMark.None, CellMark.Highlight, CellMark.Prime },
		},
		Badges = new[]
		{
			new[] { "<span style='color:#2e7d32'>★</span>", null, null },
			new string[3],
			new[] { null, null, "<span style='color:#e65100'>′</span>" },
		},
		RowCovered = new[] { false, true, false },
		ColCovered = new[] { true, false, false },
		Footer = "示例：★=星标零　′=打′零　黄=高亮　红线=覆盖线",
	};
	MatrixRenderer.Render(vis).Dump();
}

public enum CellMark
{
	None,       // 无
	Highlight,  // 浅黄：一般高亮（如新出现的零）
	Star,       // 绿：星标零（当前指派）
	Prime,      // 橙：打 ′ 的零（候选）
	Current,    // 深黄 + 橙框：当前正在处理的单元格
	Conflict,   // 红：冲突
}

public class MatrixVis
{
	public string[] RowLabels { get; set; }
	public string[] ColLabels { get; set; }
	public double[][] Values { get; set; }        // 必填
	public string[][] SubTexts { get; set; }      // 可空：单元格下方小字
	public CellMark[][] Marks { get; set; }       // 可空：底色标记
	public string[][] Badges { get; set; }        // 可空：数值后的角标（原样输出 HTML，如 ★）
	public bool[] RowCovered { get; set; }        // 可空：行覆盖线
	public bool[] ColCovered { get; set; }        // 可空：列覆盖线
	public string Footer { get; set; }            // 可空：表格下方图例
}

public static class MatrixRenderer
{
	// 覆盖线：用背景渐变画一条贯穿的半透明红线
	const string CoverH = "linear-gradient(to bottom, transparent 46%, rgba(211,47,47,.55) 46%, rgba(211,47,47,.55) 54%, transparent 54%)";
	const string CoverV = "linear-gradient(to right, transparent 46%, rgba(211,47,47,.55) 46%, rgba(211,47,47,.55) 54%, transparent 54%)";

	public static object Render(MatrixVis m)
	{
		int rows = m.Values.Length, cols = m.Values[0].Length;
		var sb = new StringBuilder();
		sb.Append("<div style='font-family:Consolas,\"Microsoft YaHei\",monospace'>");
		sb.Append("<table style='border-collapse:collapse;margin:8px 0'>");

		// 表头
		sb.Append("<tr><th></th>");
		for (int c = 0; c < cols; c++)
		{
			bool cov = m.ColCovered != null && m.ColCovered[c];
			sb.Append($"<th style='padding:4px 8px;font-size:13px;color:{(cov ? "#d32f2f" : "#556")}'>{Enc(Label(m.ColLabels, c, "C"))}</th>");
		}
		sb.Append("</tr>");

		for (int r = 0; r < rows; r++)
		{
			bool rowCov = m.RowCovered != null && m.RowCovered[r];
			sb.Append("<tr>");
			sb.Append($"<th style='padding:4px 8px;font-size:13px;text-align:right;color:{(rowCov ? "#d32f2f" : "#556")}'>{Enc(Label(m.RowLabels, r, "R"))}</th>");

			for (int c = 0; c < cols; c++)
			{
				var mark = m.Marks?[r][c] ?? CellMark.None;
				string bg = mark switch
				{
					CellMark.Highlight => "#fff9c4",
					CellMark.Star      => "#c8f7c5",
					CellMark.Prime     => "#ffe0b2",
					CellMark.Current   => "#ffe082",
					CellMark.Conflict  => "#ffcdd2",
					_                  => "#fdfdfd",
				};
				string outline = mark == CellMark.Current ? "outline:2px solid #f57f17;outline-offset:-2px;" : "";

				var lines = new List<string>();
				if (rowCov) lines.Add(CoverH);
				if (m.ColCovered != null && m.ColCovered[c]) lines.Add(CoverV);
				string bgImg = lines.Count > 0 ? $"background-image:{string.Join(",", lines)};" : "";

				string badge = m.Badges?[r][c] is string b && b.Length > 0 ? $"<span style='font-weight:bold'> {b}</span>" : "";
				string sub = m.SubTexts?[r][c];

				sb.Append($"<td style='border:1px solid #c9c9c9;min-width:56px;text-align:center;padding:3px 6px;color:#222;background-color:{bg};{bgImg}{outline}'>");
				sb.Append($"<div style='font-size:15px'>{FormatVal(m.Values[r][c])}{badge}</div>");
				if (sub != null) sb.Append($"<div style='font-size:10px;color:#999'>{Enc(sub)}</div>");
				sb.Append("</td>");
			}
			sb.Append("</tr>");
		}
		sb.Append("</table>");
		if (m.Footer != null)
			sb.Append($"<div style='font-size:12px;color:#777;margin-top:2px'>{Enc(m.Footer)}</div>");
		sb.Append("</div>");
		return Util.RawHtml(sb.ToString());
	}

	static string Label(string[] labels, int i, string fallback)
		=> labels != null && i < labels.Length ? labels[i] : $"{fallback}{i + 1}";

	static string FormatVal(double v)
		=> double.IsPositiveInfinity(v) ? "∞"
		 : v == Math.Floor(v) ? ((long)v).ToString()
		 : v.ToString("0.##");

	static string Enc(string s) => System.Net.WebUtility.HtmlEncode(s ?? "");
}

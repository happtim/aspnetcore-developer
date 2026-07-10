<Query Kind="Program" />

// ============================================================
// 图渲染器（SVG），Dijkstra 等图类算法使用。
// 节点：圆圈 + 编号 + 副标签（如当前距离）；边：线段 + 权重标签。
// 颜色/线宽由调用方在 VisNode / VisEdge 上设置，本文件只负责画。
// 边按 Width 升序绘制，粗的（重要的）画在上层。
// 直接 F5 运行本文件可看渲染示例。
// ============================================================

void Main()
{
	// 渲染示例：三角形小图
	var nodes = new[]
	{
		new VisNode { Id = "A", X = 100, Y = 200, Fill = "#ffd54f", SubLabel = "0" },
		new VisNode { Id = "B", X = 300, Y = 80,  SubLabel = "4" },
		new VisNode { Id = "C", X = 300, Y = 320, Fill = "#e0e0e0", SubLabel = "2" },
	};
	var edges = new[]
	{
		new VisEdge { From = "A", To = "B", Label = "4", Stroke = "#ef6c00", Width = 3 },
		new VisEdge { From = "A", To = "C", Label = "2" },
		new VisEdge { From = "B", To = "C", Label = "1" },
	};
	GraphRenderer.Render(nodes, edges, 420, 400, "示例：橙色边正在被处理").Dump();
}

public class VisNode
{
	public string Id { get; set; }
	public double X { get; set; }
	public double Y { get; set; }
	public string Fill { get; set; } = "#ffffff";
	public string Stroke { get; set; } = "#555555";
	public double StrokeW { get; set; } = 1.5;
	public string SubLabel { get; set; }          // 节点内下方小字（如当前最短距离）
}

public class VisEdge
{
	public string From { get; set; }
	public string To { get; set; }
	public string Label { get; set; }             // 边中点标签（如权重）
	public string Stroke { get; set; } = "#bbbbbb";
	public double Width { get; set; } = 1.5;
}

public static class GraphRenderer
{
	public static object Render(IEnumerable<VisNode> nodes, IEnumerable<VisEdge> edges,
		int width = 640, int height = 400, string footer = null)
	{
		var ns = nodes.ToList();
		var es = edges.OrderBy(e => e.Width).ToList();   // 粗边后画，盖住细边
		var map = ns.ToDictionary(n => n.Id);
		var sb = new StringBuilder();

		sb.Append("<div style='font-family:Consolas,\"Microsoft YaHei\",monospace'>");
		sb.Append($"<svg width='{width}' height='{height}' style='background:#fafafa;border:1px solid #ddd;border-radius:6px'>");

		foreach (var e in es)
		{
			var a = map[e.From];
			var b = map[e.To];
			sb.Append($"<line x1='{(int)a.X}' y1='{(int)a.Y}' x2='{(int)b.X}' y2='{(int)b.Y}' stroke='{e.Stroke}' stroke-width='{N(e.Width)}'/>");
		}

		// 权重标签统一后画，避免被边盖住；沿法线方向偏移一点
		foreach (var e in es.Where(e => e.Label != null))
		{
			var a = map[e.From];
			var b = map[e.To];
			double mx = (a.X + b.X) / 2, my = (a.Y + b.Y) / 2;
			double dx = b.X - a.X, dy = b.Y - a.Y, len = Math.Sqrt(dx * dx + dy * dy);
			if (len > 0) { mx += -dy / len * 12; my += dx / len * 12; }
			sb.Append($"<text x='{(int)mx}' y='{(int)my}' font-size='12' text-anchor='middle' fill='#333' style='paint-order:stroke;stroke:#fafafa;stroke-width:4px'>{Enc(e.Label)}</text>");
		}

		foreach (var n in ns)
		{
			sb.Append($"<circle cx='{(int)n.X}' cy='{(int)n.Y}' r='20' fill='{n.Fill}' stroke='{n.Stroke}' stroke-width='{N(n.StrokeW)}'/>");
			sb.Append($"<text x='{(int)n.X}' y='{(int)n.Y - 1}' font-size='13' font-weight='bold' text-anchor='middle' fill='#222'>{Enc(n.Id)}</text>");
			if (n.SubLabel != null)
				sb.Append($"<text x='{(int)n.X}' y='{(int)n.Y + 12}' font-size='10' text-anchor='middle' fill='#1565c0'>{Enc(n.SubLabel)}</text>");
		}

		sb.Append("</svg>");
		if (footer != null)
			sb.Append($"<div style='font-size:12px;color:#777;margin-top:4px;max-width:{width}px'>{Enc(footer)}</div>");
		sb.Append("</div>");
		return Util.RawHtml(sb.ToString());
	}

	static string N(double v) => v.ToString("0.##");
	static string Enc(string s) => System.Net.WebUtility.HtmlEncode(s ?? "");
}

<Query Kind="Program" />

#load "..\Common\StepPlayer.linq"
#load "..\Common\GraphRenderer.linq"

// ============================================================
// Dijkstra 最短路径可视化（无向带权图）
// 每轮：从未确定节点中取距离最小者标记为"已确定"，然后松弛其邻边。
// 逐步展示：当前节点（黄）、已确定集合（灰）、正在松弛的边（橙）、
//          当前最短路径树（蓝）、最终路径（绿）、节点下方=当前最短距离。
// 图数据可与 Algorithm\QuikGraph\ShortestPathsDijkstra.linq 交叉验证。
// ============================================================

void Main()
{
	var g = new WeightedGraph();
	g.AddNode("A",  70, 200);
	g.AddNode("B", 230,  90);
	g.AddNode("C", 230, 310);
	g.AddNode("D", 400,  90);
	g.AddNode("E", 400, 310);
	g.AddNode("F", 560, 200);

	g.AddEdge("A", "B", 4);
	g.AddEdge("A", "C", 2);
	g.AddEdge("B", "C", 1);
	g.AddEdge("B", "D", 5);
	g.AddEdge("C", "D", 8);
	g.AddEdge("C", "E", 10);
	g.AddEdge("D", "E", 2);
	g.AddEdge("D", "F", 6);
	g.AddEdge("E", "F", 5);

	string start = "A", goal = "F";
	var solver = new DijkstraSolver(g, start, goal);
	solver.Run();

	new StepPlayer<DijkstraState>(solver.Recorder.Steps, s => Render(s.State, g, start, goal)).Show();
}

static object Render(DijkstraState st, WeightedGraph g, string start, string goal)
{
	bool OnPath(string a, string b)
	{
		for (int i = 0; i + 1 < st.Path.Count; i++)
			if ((st.Path[i] == a && st.Path[i + 1] == b) || (st.Path[i] == b && st.Path[i + 1] == a))
				return true;
		return false;
	}

	var edges = new List<VisEdge>();
	foreach (var (a, b, w) in g.Edges)
	{
		string stroke = "#bbbbbb";
		double width = 1.5;
		if (st.Prev.GetValueOrDefault(b) == a || st.Prev.GetValueOrDefault(a) == b) { stroke = "#1976d2"; width = 3; }     // 最短路径树
		var ae = st.ActiveEdge;
		if (ae != null && ((ae.Value.A == a && ae.Value.B == b) || (ae.Value.A == b && ae.Value.B == a))) { stroke = "#ef6c00"; width = 4; }  // 正在松弛
		if (OnPath(a, b)) { stroke = "#2e7d32"; width = 5; }                                                               // 最终路径
		edges.Add(new VisEdge { From = a, To = b, Label = F(w), Stroke = stroke, Width = width });
	}

	var nodes = new List<VisNode>();
	foreach (var kv in g.Nodes)
	{
		string id = kv.Key;
		string fill = "#ffffff";
		if (st.Path.Contains(id)) fill = "#a5d6a7";
		else if (id == st.Current) fill = "#ffd54f";
		else if (st.Done.Contains(id)) fill = "#e0e0e0";
		double d = st.Dist.GetValueOrDefault(id, double.PositiveInfinity);
		nodes.Add(new VisNode
		{
			Id = id,
			X = kv.Value.X,
			Y = kv.Value.Y,
			Fill = fill,
			Stroke = id == start ? "#2e7d32" : id == goal ? "#c62828" : "#555555",
			StrokeW = id == start || id == goal ? 3 : 1.5,
			SubLabel = double.IsInfinity(d) ? "∞" : F(d),
		});
	}

	return GraphRenderer.Render(nodes, edges, 640, 400,
		"绿框=起点　红框=终点　黄=当前节点　灰=已确定　橙边=正在松弛　蓝边=最短路径树　绿边=最终路径　节点内小字=当前最短距离");
}

static string F(double v) => v == Math.Floor(v) ? ((long)v).ToString() : v.ToString("0.##");

public class WeightedGraph
{
	public Dictionary<string, (double X, double Y)> Nodes { get; } = new();
	public List<(string A, string B, double W)> Edges { get; } = new();

	public void AddNode(string id, double x, double y) => Nodes[id] = (x, y);
	public void AddEdge(string a, string b, double w) => Edges.Add((a, b, w));   // 无向边

	public IEnumerable<(string To, double W)> Neighbors(string id)
	{
		foreach (var e in Edges)
		{
			if (e.A == id) yield return (e.B, e.W);
			else if (e.B == id) yield return (e.A, e.W);
		}
	}
}

public class DijkstraState
{
	public Dictionary<string, double> Dist { get; set; }
	public Dictionary<string, string> Prev { get; set; }
	public HashSet<string> Done { get; set; }
	public string Current { get; set; }
	public (string A, string B)? ActiveEdge { get; set; }
	public List<string> Path { get; set; } = new();
}

public class DijkstraSolver
{
	public StepRecorder<DijkstraState> Recorder { get; } = new();

	readonly WeightedGraph _g;
	readonly string _start, _goal;
	readonly Dictionary<string, double> _dist = new();
	readonly Dictionary<string, string> _prev = new();
	readonly HashSet<string> _done = new();

	public DijkstraSolver(WeightedGraph g, string start, string goal)
	{
		_g = g;
		_start = start;
		_goal = goal;
	}

	public void Run()
	{
		foreach (var id in _g.Nodes.Keys) _dist[id] = double.PositiveInfinity;
		_dist[_start] = 0;
		Record("初始化", $"起点 {_start} 的距离设为 0，其余节点为 ∞；每轮取未确定节点中距离最小者");

		while (true)
		{
			// 取未确定节点中距离最小的（小图用线性扫描，步骤更直观）
			string u = null;
			double best = double.PositiveInfinity;
			foreach (var kv in _dist)
				if (!_done.Contains(kv.Key) && kv.Value < best) { best = kv.Value; u = kv.Key; }

			if (u == null)
			{
				Record("结束", "剩余节点都不可达，算法结束");
				break;
			}

			_done.Add(u);

			if (u == _goal)
			{
				var path = new List<string>();
				for (string p = _goal; p != null; p = _prev.GetValueOrDefault(p)) path.Add(p);
				path.Reverse();
				Record("完成", $"终点 {_goal} 已确定！最短路径：{string.Join(" → ", path)}，总距离 = {F(best)}", u, null, path);
				break;
			}

			Record("选择", $"未确定节点中 {u} 的距离最小（{F(best)}）→ 标记为已确定，开始松弛它的邻边", u);

			foreach (var (v, w) in _g.Neighbors(u))
			{
				if (_done.Contains(v)) continue;
				double nd = best + w;
				if (nd < _dist[v])
				{
					string old = double.IsInfinity(_dist[v]) ? "∞" : F(_dist[v]);
					_dist[v] = nd;
					_prev[v] = u;
					Record("松弛", $"边 {u}–{v}（权 {F(w)}）：{F(best)} + {F(w)} = {F(nd)} < {old}，更新 {v} 的距离为 {F(nd)}", u, (u, v));
				}
				else
					Record("松弛", $"边 {u}–{v}（权 {F(w)}）：{F(best)} + {F(w)} = {F(nd)} ≥ {F(_dist[v])}，不更新", u, (u, v));
			}
		}
	}

	void Record(string phase, string desc, string current = null, (string, string)? activeEdge = null, List<string> path = null)
		=> Recorder.Record(phase, desc, new DijkstraState
		{
			Dist = new Dictionary<string, double>(_dist),
			Prev = new Dictionary<string, string>(_prev),
			Done = new HashSet<string>(_done),
			Current = current,
			ActiveEdge = activeEdge,
			Path = path ?? new List<string>(),
		});

	static string F(double v) => v == Math.Floor(v) ? ((long)v).ToString() : v.ToString("0.##");
}

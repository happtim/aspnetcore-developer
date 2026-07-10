<Query Kind="Program" />

#load "..\Common\StepPlayer.linq"
#load "..\Common\GridRenderer.linq"

// ============================================================
// A* 网格寻路可视化（4 方向移动，步长 1，曼哈顿距离启发）
// 每轮：从 open 集取 f = g + h 最小的格子扩展，移入 closed，更新邻居。
// 逐步展示：open（蓝）、closed（灰）、当前扩展（黄）、本步更新的邻居（橙框）、
//          最终路径（绿）；格子角标 g=已走代价 h=启发估计 f=g+h。
// 地图用字符串描述：S=起点 G=终点 #=墙 .=空地，可自由修改。
// ============================================================

void Main()
{
	var map = new[]
	{
		"............",
		".S....#.....",
		"......#.....",
		"......#.....",
		"......#.....",
		"......#..G..",
		"......#.....",
		"............",
	};

	var solver = new AStarSolver(map);
	solver.Run();

	new StepPlayer<AStarState>(solver.Recorder.Steps, s => Render(s.State, solver)).Show();
}

static object Render(AStarState st, AStarSolver s)
{
	var cells = new GridCell[s.Rows][];
	for (int r = 0; r < s.Rows; r++)
	{
		cells[r] = new GridCell[s.Cols];
		for (int c = 0; c < s.Cols; c++)
		{
			var cell = new GridCell();
			cells[r][c] = cell;
			if (s.Wall[r][c]) { cell.Fill = "#455a64"; continue; }

			if (st.Closed[r][c]) cell.Fill = "#e0e0e0";
			if (st.Open[r][c])   cell.Fill = "#bbdefb";
			if (st.Path.Contains((r, c))) cell.Fill = "#a5d6a7";
			if (st.Current.HasValue && st.Current.Value == (r, c)) cell.Fill = "#ffd54f";
			if (st.Updated.Contains((r, c))) { cell.Stroke = "#ef6c00"; cell.StrokeW = 2.5; }

			if (!double.IsInfinity(st.G[r][c]))
			{
				int g = (int)st.G[r][c], h = s.H(r, c);
				cell.TL = $"g{g}";
				cell.TR = $"h{h}";
				cell.BC = $"f{g + h}";
			}
			if ((r, c) == s.Start) { cell.Center = "S"; cell.TextColor = "#1b5e20"; }
			if ((r, c) == s.Goal)  { cell.Center = "G"; cell.TextColor = "#b71c1c"; }
		}
	}
	return GridRenderer.Render(cells, 52,
		"S=起点　G=终点　深灰=墙　蓝=open（待探索）　灰=closed（已探索）　黄=当前扩展　橙框=本步更新的邻居　绿=最终路径　g=已走代价　h=曼哈顿估计　f=g+h");
}

public class AStarState
{
	public double[][] G { get; set; }
	public bool[][] Open { get; set; }
	public bool[][] Closed { get; set; }
	public (int r, int c)? Current { get; set; }
	public List<(int r, int c)> Updated { get; set; } = new();
	public List<(int r, int c)> Path { get; set; } = new();
}

public class AStarSolver
{
	public StepRecorder<AStarState> Recorder { get; } = new();
	public int Rows { get; }
	public int Cols { get; }
	public bool[][] Wall { get; }
	public (int r, int c) Start { get; private set; }
	public (int r, int c) Goal { get; private set; }

	readonly double[][] _g;
	readonly bool[][] _open;
	readonly bool[][] _closed;
	readonly (int r, int c)?[][] _came;

	public AStarSolver(string[] map)
	{
		Rows = map.Length;
		Cols = map[0].Length;
		Wall = new bool[Rows][];
		_g = new double[Rows][];
		_open = new bool[Rows][];
		_closed = new bool[Rows][];
		_came = new (int, int)?[Rows][];
		for (int r = 0; r < Rows; r++)
		{
			Wall[r] = new bool[Cols];
			_g[r] = new double[Cols];
			_open[r] = new bool[Cols];
			_closed[r] = new bool[Cols];
			_came[r] = new (int, int)?[Cols];
			for (int c = 0; c < Cols; c++)
			{
				_g[r][c] = double.PositiveInfinity;
				switch (map[r][c])
				{
					case '#': Wall[r][c] = true; break;
					case 'S': Start = (r, c); break;
					case 'G': Goal = (r, c); break;
				}
			}
		}
	}

	/// <summary>启发函数：曼哈顿距离（4 方向、步长 1 时可采纳，保证最优）。</summary>
	public int H(int r, int c) => Math.Abs(r - Goal.r) + Math.Abs(c - Goal.c);

	public void Run()
	{
		_g[Start.r][Start.c] = 0;
		_open[Start.r][Start.c] = true;
		Record("初始化", $"起点 S({Start.r},{Start.c}) 加入 open 集：g=0，h={H(Start.r, Start.c)}，f={H(Start.r, Start.c)}", Start);

		var dirs = new (int dr, int dc)[] { (-1, 0), (1, 0), (0, -1), (0, 1) };
		while (true)
		{
			// 取 open 中 f 最小的格子（f 相同优先 h 小的，更靠近终点）
			(int r, int c)? cur = null;
			double bestF = double.MaxValue;
			int bestH = int.MaxValue;
			for (int r = 0; r < Rows; r++)
				for (int c = 0; c < Cols; c++)
				{
					if (!_open[r][c]) continue;
					double f = _g[r][c] + H(r, c);
					int hh = H(r, c);
					if (f < bestF - 1e-9 || (Math.Abs(f - bestF) < 1e-9 && hh < bestH))
					{
						bestF = f;
						bestH = hh;
						cur = (r, c);
					}
				}

			if (cur == null)
			{
				Record("失败", "open 集已空，起点到终点不可达", null);
				return;
			}

			var (cr, cc) = cur.Value;
			_open[cr][cc] = false;
			_closed[cr][cc] = true;

			if ((cr, cc) == Goal)
			{
				var path = new List<(int r, int c)>();
				(int r, int c)? p = Goal;
				while (p != null)
				{
					path.Add(p.Value);
					p = _came[p.Value.r][p.Value.c];
				}
				path.Reverse();
				Record("完成", $"到达终点 G！路径长度 = {path.Count - 1} 步（g={(int)_g[cr][cc]}），沿 came-from 回溯出绿色路径", (cr, cc), null, path);
				return;
			}

			var updated = new List<(int r, int c)>();
			foreach (var (dr, dc) in dirs)
			{
				int nr = cr + dr, nc = cc + dc;
				if (nr < 0 || nr >= Rows || nc < 0 || nc >= Cols) continue;
				if (Wall[nr][nc] || _closed[nr][nc]) continue;
				double ng = _g[cr][cc] + 1;
				if (ng < _g[nr][nc])
				{
					_g[nr][nc] = ng;
					_came[nr][nc] = (cr, cc);
					_open[nr][nc] = true;
					updated.Add((nr, nc));
				}
			}
			Record("扩展", $"open 中 f 最小的是 ({cr},{cc})：g={(int)_g[cr][cc]}，h={H(cr, cc)}，f={(int)bestF} → 移入 closed，更新 {updated.Count} 个邻居", (cr, cc), updated);
		}
	}

	void Record(string phase, string desc, (int r, int c)? current, List<(int r, int c)> updated = null, List<(int r, int c)> path = null)
		=> Recorder.Record(phase, desc, new AStarState
		{
			G = _g.Select(x => x.ToArray()).ToArray(),
			Open = _open.Select(x => x.ToArray()).ToArray(),
			Closed = _closed.Select(x => x.ToArray()).ToArray(),
			Current = current,
			Updated = updated ?? new List<(int r, int c)>(),
			Path = path ?? new List<(int r, int c)>(),
		});
}

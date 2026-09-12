<Query Kind="Program" />

#load "..\Common\StepPlayer.linq"
#load "..\Common\GridRenderer.linq"

// ============================================================
// Dijkstra 网格寻路可视化（4 方向移动，格子带地形代价）
// 每轮：从 open 集取 d（起点到该格的已知最短代价）最小的格子扩展，移入 closed，松弛邻居。
// 与 A* 的唯一区别：没有启发值 h（等价于 h=0 的 A*），所以它向四周均匀扩散，
// 扩展的格子更多，但不需要任何关于终点的先验知识，且天然支持带权地形。
// 逐步展示：open（蓝）、closed（灰）、当前扩展（黄）、本步更新的邻居（橙框）、
//          最终路径（绿）；格子右上角=进入该格的代价（仅非 1 时显示），底部=d 值。
// 地图用字符串描述：S=起点 G=终点 #=墙 .=平地(代价 1) ~=沼泽(代价 3)，可自由修改。
// 与 AStar\AStar.linq 使用同一张底图，可对比两者扩展的格子数量。
// ============================================================

void Main()
{
	var map = new[]
	{
		"............",
		".S....#.....",
		"......#.....",
		"..~~~.#.....",
		"..~~~.#.....",
		"..~~~.#..G..",
		"......#.....",
		"............",
	};

	var solver = new DijkstraSolver(map);
	solver.Run();

	new StepPlayer<DijkstraState>(solver.Recorder.Steps, s => Render(s.State, solver)).Show();
}

static object Render(DijkstraState st, DijkstraSolver s)
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

			if (s.Cost[r][c] > 1) cell.Fill = "#fff3e0";                       // 沼泽底色（淡橙）
			if (st.Closed[r][c]) cell.Fill = "#e0e0e0";
			if (st.Open[r][c])   cell.Fill = "#bbdefb";
			if (st.Path.Contains((r, c))) cell.Fill = "#a5d6a7";
			if (st.Current.HasValue && st.Current.Value == (r, c)) cell.Fill = "#ffd54f";
			if (st.Updated.Contains((r, c))) { cell.Stroke = "#ef6c00"; cell.StrokeW = 2.5; }

			if (s.Cost[r][c] > 1) cell.TR = $"×{s.Cost[r][c]}";
			if (!double.IsInfinity(st.Dist[r][c])) cell.BC = $"d{(int)st.Dist[r][c]}";
			if ((r, c) == s.Start) { cell.Center = "S"; cell.TextColor = "#1b5e20"; }
			if ((r, c) == s.Goal)  { cell.Center = "G"; cell.TextColor = "#b71c1c"; }
		}
	}
	return GridRenderer.Render(cells, 52,
		"S=起点　G=终点　深灰=墙　淡橙=沼泽(进入代价 3)　蓝=open（待探索）　灰=closed（已确定）　黄=当前扩展　橙框=本步更新的邻居　绿=最终路径　d=起点到该格的最短代价");
}

public class DijkstraState
{
	public double[][] Dist { get; set; }
	public bool[][] Open { get; set; }
	public bool[][] Closed { get; set; }
	public (int r, int c)? Current { get; set; }
	public List<(int r, int c)> Updated { get; set; } = new();
	public List<(int r, int c)> Path { get; set; } = new();
}

public class DijkstraSolver
{
	public StepRecorder<DijkstraState> Recorder { get; } = new();
	public int Rows { get; }
	public int Cols { get; }
	public bool[][] Wall { get; }
	public int[][] Cost { get; }                       // 进入该格的代价
	public (int r, int c) Start { get; private set; }
	public (int r, int c) Goal { get; private set; }

	readonly double[][] _dist;
	readonly bool[][] _open;
	readonly bool[][] _closed;
	readonly (int r, int c)?[][] _came;

	public DijkstraSolver(string[] map)
	{
		Rows = map.Length;
		Cols = map[0].Length;
		Wall = new bool[Rows][];
		Cost = new int[Rows][];
		_dist = new double[Rows][];
		_open = new bool[Rows][];
		_closed = new bool[Rows][];
		_came = new (int, int)?[Rows][];
		for (int r = 0; r < Rows; r++)
		{
			Wall[r] = new bool[Cols];
			Cost[r] = new int[Cols];
			_dist[r] = new double[Cols];
			_open[r] = new bool[Cols];
			_closed[r] = new bool[Cols];
			_came[r] = new (int, int)?[Cols];
			for (int c = 0; c < Cols; c++)
			{
				_dist[r][c] = double.PositiveInfinity;
				Cost[r][c] = 1;
				switch (map[r][c])
				{
					case '#': Wall[r][c] = true; break;
					case '~': Cost[r][c] = 3; break;
					case 'S': Start = (r, c); break;
					case 'G': Goal = (r, c); break;
				}
			}
		}
	}

	public void Run()
	{
		_dist[Start.r][Start.c] = 0;
		_open[Start.r][Start.c] = true;
		Record("初始化", $"起点 S({Start.r},{Start.c}) 加入 open 集，d=0；其余格子 d=∞。每轮取 open 中 d 最小的格子（没有启发值 h）", Start);

		var dirs = new (int dr, int dc)[] { (-1, 0), (1, 0), (0, -1), (0, 1) };
		int expanded = 0;
		while (true)
		{
			// 取 open 中 d 最小的格子（小网格用线性扫描，步骤更直观；实际实现用优先队列）
			(int r, int c)? cur = null;
			double best = double.PositiveInfinity;
			for (int r = 0; r < Rows; r++)
				for (int c = 0; c < Cols; c++)
					if (_open[r][c] && _dist[r][c] < best) { best = _dist[r][c]; cur = (r, c); }

			if (cur == null)
			{
				Record("失败", "open 集已空，起点到终点不可达", null);
				return;
			}

			var (cr, cc) = cur.Value;
			_open[cr][cc] = false;
			_closed[cr][cc] = true;
			expanded++;

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
				Record("完成", $"终点 G 出队，即其 d={(int)best} 已是最短代价！路径 {path.Count - 1} 步，共扩展 {expanded} 个格子（对比 A* 在同一张图上的扩展数）", (cr, cc), null, path);
				return;
			}

			var updated = new List<(int r, int c)>();
			foreach (var (dr, dc) in dirs)
			{
				int nr = cr + dr, nc = cc + dc;
				if (nr < 0 || nr >= Rows || nc < 0 || nc >= Cols) continue;
				if (Wall[nr][nc] || _closed[nr][nc]) continue;
				double nd = best + Cost[nr][nc];          // 松弛：经由当前格进入邻居的代价
				if (nd < _dist[nr][nc])
				{
					_dist[nr][nc] = nd;
					_came[nr][nc] = (cr, cc);
					_open[nr][nc] = true;
					updated.Add((nr, nc));
				}
			}
			Record("扩展", $"open 中 d 最小的是 ({cr},{cc})，d={(int)best} → 移入 closed（最短代价已确定），松弛后更新 {updated.Count} 个邻居", (cr, cc), updated);
		}
	}

	void Record(string phase, string desc, (int r, int c)? current, List<(int r, int c)> updated = null, List<(int r, int c)> path = null)
		=> Recorder.Record(phase, desc, new DijkstraState
		{
			Dist = _dist.Select(x => x.ToArray()).ToArray(),
			Open = _open.Select(x => x.ToArray()).ToArray(),
			Closed = _closed.Select(x => x.ToArray()).ToArray(),
			Current = current,
			Updated = updated ?? new List<(int r, int c)>(),
			Path = path ?? new List<(int r, int c)>(),
		});
}

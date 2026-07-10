<Query Kind="Program" />

#load "..\Common\StepPlayer.linq"
#load "..\Common\MatrixRenderer.linq"

// ============================================================
// 匈牙利算法（Kuhn-Munkres）任务分配可视化
// 行 = 机器人，列 = 任务，单元格 = 完成成本，求总成本最小的一对一指派。
// - 支持非方阵：自动补 0 成本的虚拟机器人/任务
// - 每个关键动作录制一帧快照，用 StepPlayer 逐步回放
// - 结果用暴力全排列验证是否最优
// 图例：★=当前指派的零　′=候选零　红线=覆盖线　小字=原始成本
// ============================================================

void Main()
{
	// 机器人 × 任务成本矩阵（可自由修改）
	double[][] cost =
	{
		//            T1  T2  T3  T4  T5
		new double[] {  9, 11, 14, 11,  7 },   // R1
		new double[] {  6, 15, 13, 13, 10 },   // R2
		new double[] { 12, 13,  6,  8,  8 },   // R3
		new double[] { 11,  9, 10, 12,  9 },   // R4
		new double[] {  7, 12, 14, 10, 14 },   // R5
	};
	// 非方阵示例（3 个机器人 5 个任务，自动补 2 个虚拟机器人）：
	// double[][] cost = { new double[]{9,11,14,11,7}, new double[]{6,15,13,13,10}, new double[]{12,13,6,8,8} };
	// 随机示例：
	// var rnd = new Random(42);
	// double[][] cost = Enumerable.Range(0, 6).Select(_ => Enumerable.Range(0, 6).Select(__ => (double)rnd.Next(1, 20)).ToArray()).ToArray();

	var robots = Enumerable.Range(1, cost.Length).Select(i => $"R{i}").ToArray();
	var tasks  = Enumerable.Range(1, cost[0].Length).Select(i => $"T{i}").ToArray();

	var solver = new HungarianSolver(cost, robots, tasks);
	int[] assign = solver.Solve();

	// —— 结果汇总 ——
	var summary = new List<object>();
	for (int r = 0; r < cost.Length; r++)
	{
		bool real = assign[r] >= 0 && assign[r] < cost[0].Length;
		summary.Add(new
		{
			机器人 = robots[r],
			任务   = real ? tasks[assign[r]] : "（未分配）",
			成本   = real ? cost[r][assign[r]] : 0,
		});
	}
	summary.Dump($"最终指派　总成本 = {HungarianSolver.F(solver.Total)}");

	// —— 暴力全排列验证 ——
	int n = Math.Max(cost.Length, cost[0].Length);
	if (n <= 9)
	{
		var (bfBest, _) = BruteForce(cost);
		bool ok = Math.Abs(bfBest - solver.Total) < 1e-9;
		Util.RawHtml($"<div style='font-family:Consolas,\"Microsoft YaHei\";padding:6px 10px;margin:4px 0;" +
			$"background:{(ok ? "#e8f5e9" : "#ffebee")};border-left:4px solid {(ok ? "#2e7d32" : "#c62828")};color:#222'>" +
			$"暴力枚举最优总成本 = {HungarianSolver.F(bfBest)} —— " +
			(ok ? "✓ 与匈牙利算法一致，结果最优" : "✗ 不一致，算法有 bug！") + "</div>").Dump();
	}
	else
		$"矩阵规模 {n}×{n} 太大，跳过暴力验证".Dump();

	// —— 逐步回放 ——
	new StepPlayer<HungarianState>(solver.Recorder.Steps, s => RenderStep(s, solver)).Show();
}

// 把一帧算法状态映射成 MatrixVis 交给渲染器
static object RenderStep(Step<HungarianState> step, HungarianSolver s)
{
	var st = step.State;
	int n = st.M.Length;
	var marks  = new CellMark[n][];
	var badges = new string[n][];
	var subs   = new string[n][];
	for (int r = 0; r < n; r++)
	{
		marks[r] = new CellMark[n];
		badges[r] = new string[n];
		subs[r] = new string[n];
		for (int c = 0; c < n; c++)
		{
			subs[r][c] = $"原 {HungarianSolver.F(s.Orig[r][c])}";
			if (st.Mask[r][c] == 1) { marks[r][c] = CellMark.Star;  badges[r][c] = "<span style='color:#2e7d32'>★</span>"; }
			if (st.Mask[r][c] == 2) { marks[r][c] = CellMark.Prime; badges[r][c] = "<span style='color:#e65100'>′</span>"; }
		}
	}
	foreach (var (r, c) in st.Highlights)
		if (marks[r][c] == CellMark.None) marks[r][c] = CellMark.Highlight;
	if (st.Current.HasValue)
		marks[st.Current.Value.r][st.Current.Value.c] = CellMark.Current;

	return MatrixRenderer.Render(new MatrixVis
	{
		RowLabels = s.RowLabels,
		ColLabels = s.ColLabels,
		Values = st.M,
		Marks = marks,
		Badges = badges,
		SubTexts = subs,
		RowCovered = st.RowCover,
		ColCovered = st.ColCover,
		Footer = "★=当前指派　′=候选零　深黄框=当前处理　浅黄=高亮　红线=覆盖线　大字=归约后矩阵　小字=原始成本",
	});
}

// 暴力全排列求最优指派（含虚拟行/列语义：越界成本为 0）
static (double best, int[] cols) BruteForce(double[][] cost)
{
	int nr = cost.Length, nc = cost[0].Length, n = Math.Max(nr, nc);
	var used = new bool[n];
	var cur = new int[n];
	var best = new int[n];
	double bestVal = double.MaxValue;

	void Go(int r, double acc)
	{
		if (acc >= bestVal) return;   // 剪枝
		if (r == n) { bestVal = acc; Array.Copy(cur, best, n); return; }
		for (int c = 0; c < n; c++)
		{
			if (used[c]) continue;
			used[c] = true;
			cur[r] = c;
			Go(r + 1, acc + (r < nr && c < nc ? cost[r][c] : 0));
			used[c] = false;
		}
	}
	Go(0, 0);
	return (bestVal, best);
}

/// <summary>一帧快照：归约后矩阵 + 标记 + 覆盖线 + 高亮。</summary>
public class HungarianState
{
	public double[][] M { get; set; }             // 归约后的工作矩阵
	public byte[][] Mask { get; set; }            // 0=无 1=★ 2=′
	public bool[] RowCover { get; set; }
	public bool[] ColCover { get; set; }
	public (int r, int c)? Current { get; set; }  // 当前正在处理的单元格
	public List<(int r, int c)> Highlights { get; set; } = new();
}

public class HungarianSolver
{
	public StepRecorder<HungarianState> Recorder { get; } = new();
	public double[][] Orig { get; }               // 补齐后的原始矩阵
	public string[] RowLabels { get; }
	public string[] ColLabels { get; }
	public int RealRows { get; }
	public int RealCols { get; }
	public double Total { get; private set; }

	readonly int _n;
	readonly double[][] _m;                       // 工作矩阵（不断归约/调整）
	readonly byte[][] _mask;
	readonly bool[] _rowCover;
	readonly bool[] _colCover;

	public HungarianSolver(double[][] cost, string[] rowLabels, string[] colLabels)
	{
		RealRows = cost.Length;
		RealCols = cost[0].Length;
		_n = Math.Max(RealRows, RealCols);

		Orig = new double[_n][];
		_m = new double[_n][];
		_mask = new byte[_n][];
		_rowCover = new bool[_n];
		_colCover = new bool[_n];
		for (int r = 0; r < _n; r++)
		{
			Orig[r] = new double[_n];
			_m[r] = new double[_n];
			_mask[r] = new byte[_n];
			for (int c = 0; c < _n; c++)
			{
				double v = r < RealRows && c < RealCols ? cost[r][c] : 0;   // 虚拟行/列成本 0
				Orig[r][c] = v;
				_m[r][c] = v;
			}
		}
		RowLabels = Enumerable.Range(0, _n).Select(i => i < RealRows ? rowLabels[i] : $"虚R{i + 1}").ToArray();
		ColLabels = Enumerable.Range(0, _n).Select(i => i < RealCols ? colLabels[i] : $"虚T{i + 1}").ToArray();
	}

	public int[] Solve()
	{
		string padNote = _n > RealRows ? $"（机器人少于任务，补 {_n - RealRows} 个成本为 0 的虚拟机器人）"
					   : _n > RealCols ? $"（任务少于机器人，补 {_n - RealCols} 个成本为 0 的虚拟任务）"
					   : "";
		Record("初始", "原始成本矩阵" + padNote);

		// —— 1. 行归约：每行减去本行最小值 ——
		for (int r = 0; r < _n; r++)
		{
			double min = _m[r].Min();
			if (min > 0)
				for (int c = 0; c < _n; c++) _m[r][c] -= min;
			int zc = Array.FindIndex(_m[r], IsZero);
			Record("行归约",
				min > 0 ? $"{RowLabels[r]} 行每个元素减去行最小值 {F(min)}，出现零元素"
						: $"{RowLabels[r]} 行已含 0，无需归约",
				null, new[] { (r, zc) });
		}

		// —— 2. 列归约：每列减去本列最小值（合并为一步）——
		var reduced = new List<string>();
		var newZeros = new List<(int, int)>();
		for (int c = 0; c < _n; c++)
		{
			double min = double.MaxValue;
			for (int r = 0; r < _n; r++) min = Math.Min(min, _m[r][c]);
			if (min > 0)
			{
				for (int r = 0; r < _n; r++) _m[r][c] -= min;
				reduced.Add($"{ColLabels[c]} 减 {F(min)}");
				for (int r = 0; r < _n; r++) if (IsZero(_m[r][c])) newZeros.Add((r, c));
			}
		}
		Record("列归约",
			reduced.Count > 0 ? $"列归约：{string.Join("，", reduced)}" : "每列都已含 0，列归约无变化",
			null, newZeros);

		// —— 3. 初始试指派：贪心标记互不冲突的零（★）——
		var rowHas = new bool[_n];
		var colHas = new bool[_n];
		for (int r = 0; r < _n; r++)
			for (int c = 0; c < _n; c++)
				if (IsZero(_m[r][c]) && !rowHas[r] && !colHas[c])
				{
					_mask[r][c] = 1;
					rowHas[r] = colHas[c] = true;
					Record("试指派", $"{RowLabels[r]} 在零元素处暂选 {ColLabels[c]}（标记 ★，原成本 {F(Orig[r][c])}）", (r, c));
				}

		// —— 主循环 ——
		int guard = 0, guardMax = 500 * _n * _n;
		while (true)
		{
			if (++guard > guardMax) throw new Exception("迭代次数异常，算法可能有 bug");

			// 4. 覆盖所有含 ★ 的列；全覆盖则完成
			Array.Clear(_colCover, 0, _n);
			int covered = 0;
			var stars = new List<(int, int)>();
			for (int c = 0; c < _n; c++)
				for (int r = 0; r < _n; r++)
					if (_mask[r][c] == 1)
					{
						_colCover[c] = true;
						covered++;
						stars.Add((r, c));
						break;
					}
			bool done = covered == _n;
			Record("检查", $"覆盖所有含 ★ 的列：{covered}/{_n}" +
				(done ? " —— 全部覆盖，指派完成！" : "，尚未完成，继续寻找改进"), null, stars);
			if (done) break;

			// 5/6. 反复找未覆盖的零打 ′；找不到就调整矩阵制造新零
			while (true)
			{
				if (++guard > guardMax) throw new Exception("迭代次数异常，算法可能有 bug");

				var z = FindUncoveredZero();
				if (z == null)
				{
					AdjustMatrix();
					continue;
				}
				var (r, c) = z.Value;
				_mask[r][c] = 2;
				int sc = FindStarInRow(r);
				if (sc >= 0)
				{
					_rowCover[r] = true;
					_colCover[sc] = false;
					Record("换线", $"未覆盖零 ({RowLabels[r]}, {ColLabels[c]}) 标记 ′；该行已有 ★（{ColLabels[sc]}）" +
						$"→ 改为覆盖 {RowLabels[r]} 行、取消覆盖 {ColLabels[sc]} 列", (r, c));
				}
				else
				{
					Record("增广", $"未覆盖零 ({RowLabels[r]}, {ColLabels[c]}) 标记 ′；该行没有 ★ → 找到增广路，指派数可以 +1", (r, c));
					Augment(r, c);
					break;   // 回到第 4 步
				}
			}
		}

		// —— 提取结果 ——
		var result = new int[RealRows];
		var finalCells = new List<(int, int)>();
		var parts = new List<string>();
		Total = 0;
		for (int r = 0; r < RealRows; r++)
		{
			int c = Array.IndexOf(_mask[r], (byte)1);
			result[r] = c;
			if (c >= 0 && c < RealCols)
			{
				Total += Orig[r][c];
				finalCells.Add((r, c));
				parts.Add($"{RowLabels[r]}→{ColLabels[c]}({F(Orig[r][c])})");
			}
			else
				parts.Add($"{RowLabels[r]}→无任务");
		}
		Record("完成", $"最终指派：{string.Join("，", parts)}，总成本 = {F(Total)}", null, finalCells);
		return result;
	}

	// 沿交替路径增广：′ → ★ → ′ → ★ …，把 ′ 升为 ★、路径上原 ★ 取消
	void Augment(int r, int c)
	{
		var path = new List<(int r, int c)> { (r, c) };
		while (true)
		{
			int sr = FindStarInCol(path[path.Count - 1].c);
			if (sr < 0) break;
			path.Add((sr, path[path.Count - 1].c));   // 同列的 ★
			path.Add((sr, FindPrimeInRow(sr)));       // 同行的 ′（必存在）
		}
		foreach (var p in path)
			_mask[p.r][p.c] = _mask[p.r][p.c] == 1 ? (byte)0 : (byte)1;

		// 清除所有覆盖线和残留的 ′
		Array.Clear(_rowCover, 0, _n);
		Array.Clear(_colCover, 0, _n);
		for (int i = 0; i < _n; i++)
			for (int j = 0; j < _n; j++)
				if (_mask[i][j] == 2) _mask[i][j] = 0;

		Record("增广", $"沿交替路径（{path.Count} 个零）把 ′ 升为 ★、原 ★ 取消，指派数 +1；清除覆盖线和 ′", null, path);
	}

	// 没有未覆盖的零时：用最小未覆盖值调整矩阵，制造新零
	void AdjustMatrix()
	{
		double e = double.MaxValue;
		for (int r = 0; r < _n; r++)
			for (int c = 0; c < _n; c++)
				if (!_rowCover[r] && !_colCover[c]) e = Math.Min(e, _m[r][c]);

		for (int r = 0; r < _n; r++)
			for (int c = 0; c < _n; c++)
			{
				if (_rowCover[r] && _colCover[c]) _m[r][c] += e;
				else if (!_rowCover[r] && !_colCover[c]) _m[r][c] -= e;
			}

		var zeros = new List<(int, int)>();
		for (int r = 0; r < _n; r++)
			for (int c = 0; c < _n; c++)
				if (!_rowCover[r] && !_colCover[c] && IsZero(_m[r][c])) zeros.Add((r, c));

		Record("矩阵调整", $"没有未覆盖的零：取未覆盖元素最小值 {F(e)}，未覆盖元素减 {F(e)}、双重覆盖元素加 {F(e)}，制造新零", null, zeros);
	}

	(int, int)? FindUncoveredZero()
	{
		for (int r = 0; r < _n; r++)
		{
			if (_rowCover[r]) continue;
			for (int c = 0; c < _n; c++)
				if (!_colCover[c] && IsZero(_m[r][c])) return (r, c);
		}
		return null;
	}

	int FindStarInRow(int r) => Array.IndexOf(_mask[r], (byte)1);
	int FindPrimeInRow(int r) => Array.IndexOf(_mask[r], (byte)2);
	int FindStarInCol(int c)
	{
		for (int r = 0; r < _n; r++) if (_mask[r][c] == 1) return r;
		return -1;
	}

	static bool IsZero(double v) => Math.Abs(v) < 1e-9;

	void Record(string phase, string desc, (int r, int c)? current = null, IEnumerable<(int, int)> highlights = null)
		=> Recorder.Record(phase, desc, new HungarianState
		{
			M = _m.Select(row => row.ToArray()).ToArray(),
			Mask = _mask.Select(row => row.ToArray()).ToArray(),
			RowCover = (bool[])_rowCover.Clone(),
			ColCover = (bool[])_colCover.Clone(),
			Current = current,
			Highlights = highlights?.Select(t => ((int r, int c))t).ToList() ?? new(),
		});

	public static string F(double v) => v == Math.Floor(v) ? ((long)v).ToString() : v.ToString("0.##");
}

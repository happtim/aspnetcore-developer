<Query Kind="Program">
  <Namespace>LINQPad.Controls</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

// ============================================================
// 通用步骤播放器（被各算法脚本 #load 引用）
// 用法：算法运行时用 StepRecorder<TState>.Record(阶段, 说明, 状态快照)
//       录制每一步，然后 new StepPlayer<TState>(steps, 渲染函数).Show()
// 提供：上一步 / 下一步 / 自动播放 / 速度切换
// 直接 F5 运行本文件可看一个最小示例。
// ============================================================

void Main()
{
	// 最小演示：数到 5
	var rec = new StepRecorder<int>();
	for (int i = 1; i <= 5; i++)
		rec.Record("演示", $"当前数字是 {i}", i);

	new StepPlayer<int>(rec.Steps,
		s => Util.RawHtml($"<div style='font-size:48px;padding:24px;font-family:Consolas'>{s.State}</div>"))
		.Show();
}

/// <summary>一帧快照：算法在某个时刻的完整状态 + 说明文字。</summary>
public class Step<TState>
{
	public int Index { get; set; }
	public string Phase { get; set; } = "";
	public string Description { get; set; } = "";
	public TState State { get; set; }
}

/// <summary>算法侧的录制器。注意 state 必须是深拷贝的快照，录制后不能再被算法修改。</summary>
public class StepRecorder<TState>
{
	public List<Step<TState>> Steps { get; } = new();

	public void Record(string phase, string description, TState state)
		=> Steps.Add(new Step<TState> { Index = Steps.Count, Phase = phase, Description = description, State = state });
}

/// <summary>播放器：控制条 + 状态栏 + 画布（DumpContainer），每步调用渲染函数刷新。</summary>
public class StepPlayer<TState>
{
	readonly IReadOnlyList<Step<TState>> _steps;
	readonly Func<Step<TState>, object> _render;
	readonly DumpContainer _status = new();
	readonly DumpContainer _canvas = new();
	readonly int[] _delays = { 1500, 800, 400, 150 };
	readonly string[] _speedNames = { "0.5x", "1x", "2x", "4x" };
	int _speedIdx = 1;
	int _pos;
	bool _playing;
	Button _btnPlay;

	public StepPlayer(IReadOnlyList<Step<TState>> steps, Func<Step<TState>, object> render)
	{
		if (steps == null || steps.Count == 0) throw new ArgumentException("没有可播放的步骤");
		_steps = steps;
		_render = render;
	}

	public void Show()
	{
		var btnFirst = new Button("⏮ 开头", _ => { StopPlay(); Go(0); });
		var btnPrev  = new Button("◀ 上一步", _ => { StopPlay(); Go(_pos - 1); });
		_btnPlay     = new Button("▶ 自动播放", _ => TogglePlay());
		var btnNext  = new Button("下一步 ▶", _ => { StopPlay(); Go(_pos + 1); });
		var btnLast  = new Button("结尾 ⏭", _ => { StopPlay(); Go(_steps.Count - 1); });
		var btnSpeed = new Button($"速度 {_speedNames[_speedIdx]}", b =>
		{
			_speedIdx = (_speedIdx + 1) % _delays.Length;
			b.Text = $"速度 {_speedNames[_speedIdx]}";
		});

		Util.HorizontalRun(true, btnFirst, btnPrev, _btnPlay, btnNext, btnLast, btnSpeed).Dump();
		_status.Dump();
		_canvas.Dump();
		Go(0);
	}

	void Go(int i)
	{
		_pos = Math.Clamp(i, 0, _steps.Count - 1);
		var s = _steps[_pos];
		_status.Content = Util.RawHtml(
			"<div style='font-family:Consolas,\"Microsoft YaHei\",monospace;margin:6px 0;padding:6px 10px;" +
			"background:#eef2ff;border-left:4px solid #4a6fd4;color:#222'>" +
			$"<b>步骤 {_pos + 1}/{_steps.Count}</b> &nbsp; " +
			$"<span style='color:#4a6fd4'>[{Enc(s.Phase)}]</span> {Enc(s.Description)}</div>");
		_canvas.Content = _render(s);
	}

	void StopPlay()
	{
		_playing = false;
		if (_btnPlay != null) _btnPlay.Text = "▶ 自动播放";
	}

	async void TogglePlay()
	{
		if (_playing) { StopPlay(); return; }
		_playing = true;
		_btnPlay.Text = "⏸ 暂停";
		if (_pos >= _steps.Count - 1) Go(0);   // 播完了再按 → 从头播
		while (_playing && _pos < _steps.Count - 1)
		{
			await Task.Delay(_delays[_speedIdx]);
			if (!_playing) break;
			Go(_pos + 1);
		}
		StopPlay();
	}

	static string Enc(string s) => System.Net.WebUtility.HtmlEncode(s ?? "");
}

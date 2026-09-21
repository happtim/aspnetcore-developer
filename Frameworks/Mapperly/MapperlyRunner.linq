<Query Kind="Statements">
  <NuGetReference Version="4.3.1">Riok.Mapperly</NuGetReference>
  <NuGetReference Version="4.14.0">Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
  <Namespace>Riok.Mapperly.Abstractions</Namespace>
  <Namespace>System.Reflection</Namespace>
  <Namespace>System.Runtime.Loader</Namespace>
</Query>

// LINQPad 7 不会运行 Roslyn 源生成器。这里在脚本内手动驱动 Mapperly：
// 读取目标 .linq（按文档正常写 [Mapper] partial 类），用 Roslyn 编译并跑生成器，
// 把生成的 partial 实现写成 <目标>.g.linq，目标脚本 #load 它之后即可正常运行。
// 用法见 Generator.linq。
public static class MapperlyRunner
{
	// LINQPad 7 = C# 10；与其保持一致，避免生成器这边能编译而 LINQPad 里不能
	const LanguageVersion LangVersion = LanguageVersion.CSharp10;

	// LINQPad 查询默认导入的命名空间
	static readonly string[] DefaultNamespaces =
	{
		"System", "System.IO", "System.Text", "System.Text.RegularExpressions", "System.Diagnostics",
		"System.Threading", "System.Threading.Tasks", "System.Reflection", "System.Collections",
		"System.Collections.Generic", "System.Linq", "System.Linq.Expressions", "System.Data",
		"System.Xml", "System.Xml.Linq", "System.Xml.XPath", "LINQPad",
	};

	/// <summary>扫描 root 下（含子目录）所有声明了 [Mapper] 的可执行 .linq，逐个生成 .g.linq，返回每个文件的结果。</summary>
	public static List<GenerateResult> GenerateAll(string root)
	{
		var results = new List<GenerateResult>();
		foreach (var path in Directory.EnumerateFiles(root, "*.linq", SearchOption.AllDirectories).OrderBy(p => p, StringComparer.OrdinalIgnoreCase))
		{
			if (!IsMapperQuery(path)) continue;

			var relative = Path.GetRelativePath(root, path);
			try
			{
				var generated = Generate(path, out var errorIds);
				results.Add(errorIds.Count == 0
					? new GenerateResult(relative, "OK", Path.GetFileName(generated))
					: new GenerateResult(relative, "有错误", $"{string.Join(", ", errorIds)}（见上方 Diagnostics；真实项目里会编译失败）"));
			}
			catch (Exception ex)
			{
				results.Add(new GenerateResult(relative, "失败", ex.Message));
			}
		}
		return results;
	}

	public record GenerateResult(string 脚本, string 状态, string 说明);

	// 工具脚本自身不参与扫描
	static readonly string[] ToolingFiles = { "Generator.linq", "MapperlyRunner.linq" };

	// 可执行（Statements/Program）且代码里出现 [Mapper] / [Mapper(...)] 的脚本才需要生成；.g.linq 自身跳过
	static bool IsMapperQuery(string path)
	{
		if (path.EndsWith(".g.linq", StringComparison.OrdinalIgnoreCase)) return false;
		if (ToolingFiles.Contains(Path.GetFileName(path), StringComparer.OrdinalIgnoreCase)) return false;

		var (header, body) = SplitQuery(File.ReadAllText(path));
		var kind = (string?)header.Attribute("Kind");
		if (kind != "Statements" && kind != "Program") return false;

		// 去掉 // 行注释，避免注释里提到 [Mapper] 被误判
		var code = Regex.Replace(body, @"//.*$", "", RegexOptions.Multiline);
		return Regex.IsMatch(code, @"\[Mapper\s*(\]|\()");
	}

	/// <summary>读取 linqPath（及其 #load 的文件），跑生成器，把生成的代码写成同目录下的 &lt;name&gt;.g.linq，返回生成文件路径。</summary>
	public static string Generate(string linqPath) => Generate(linqPath, out _);

	/// <param name="errorIds">Error 级诊断的 Id。生成器遇到 Error 仍会产出代码，但真实项目里这会导致编译失败，不能当成 OK。</param>
	public static string Generate(string linqPath, out List<string> errorIds)
	{
		linqPath = Path.GetFullPath(linqPath);
		var outPath = Path.Combine(Path.GetDirectoryName(linqPath)!, Path.GetFileNameWithoutExtension(linqPath) + ".g.linq");

		var parseOptions = new CSharpParseOptions(LangVersion);
		var namespaces = new List<string>(DefaultNamespaces);
		var trees = new List<SyntaxTree>();
		var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

		// 递归读取 #load 的文件（模型类通常放在单独的 .linq 里），跳过本次要生成的 .g.linq
		void Load(string path)
		{
			path = Path.GetFullPath(path);
			// LINQPad 允许 #load 省略 .linq 扩展名
			if (!File.Exists(path) && !path.EndsWith(".linq", StringComparison.OrdinalIgnoreCase))
				path += ".linq";
			if (string.Equals(path, outPath, StringComparison.OrdinalIgnoreCase) || !visited.Add(path))
				return;

			var (header, body) = SplitQuery(File.ReadAllText(path));
			namespaces.AddRange(header.Descendants("Namespace").Select(e => e.Value.Trim()));

			// Statements 脚本的正文（语句在前、类型在后）本身就是合法的顶级语句程序；
			// #load 行替换为空行以保持行号一致，被引用的文件作为独立语法树加入。
			var lines = body.Split('\n');
			for (var i = 0; i < lines.Length; i++)
			{
				var m = Regex.Match(lines[i], @"^\s*#load\s+""([^""]+)""");
				if (!m.Success) continue;
				Load(Path.Combine(Path.GetDirectoryName(path)!, m.Groups[1].Value));
				lines[i] = "";
			}
			trees.Add(CSharpSyntaxTree.ParseText(string.Join("\n", lines), parseOptions, path: path));
		}
		Load(linqPath);

		// 头部 <Namespace> + LINQPad 默认命名空间 → global using
		var usings = string.Join("\n", namespaces.Distinct().Select(n => $"global using {n};"));
		trees.Insert(0, CSharpSyntaxTree.ParseText(usings, parseOptions, path: "GlobalUsings.cs"));

		var (driver, outputCompilation, generatorDiagnostics) = RunGenerators(trees.ToArray(), OutputKind.ConsoleApplication);
		var generated = driver.GetRunResult().Results.SelectMany(r => r.GeneratedSources).ToList();

		errorIds = Report(generatorDiagnostics, outputCompilation, $"Diagnostics: {Path.GetFileName(linqPath)}");

		if (generated.Count == 0)
			throw new InvalidOperationException($"{Path.GetFileName(linqPath)} 里没有产生任何生成代码，检查 [Mapper] 类是否存在及上面的诊断信息。");

		var sb = new StringBuilder();
		sb.AppendLine("<Query Kind=\"Statements\" />");
		sb.AppendLine();
		sb.AppendLine($"// 由 Generator.linq 从 {Path.GetFileName(linqPath)} 生成，请勿手改；Mapper 有改动后重新运行 Generator.linq。");
		foreach (var source in generated)
		{
			sb.AppendLine();
			sb.AppendLine($"// ---- {source.HintName} ----");
			sb.AppendLine(source.SourceText.ToString());
		}

		File.WriteAllText(outPath, sb.ToString(), new UTF8Encoding(false));
		return outPath;
	}

	/// <summary>把一段只含 Mapper 类的源码编译成程序集（脚本里已有的类型可直接引用）。</summary>
	public static Assembly Compile(string mapperSource, bool dumpGeneratedCode = true)
	{
		var tree = CSharpSyntaxTree.ParseText(mapperSource, new CSharpParseOptions(LangVersion));
		var (driver, outputCompilation, generatorDiagnostics) = RunGenerators(new[] { tree }, OutputKind.DynamicallyLinkedLibrary);

		if (dumpGeneratedCode)
			foreach (var source in driver.GetRunResult().Results.SelectMany(r => r.GeneratedSources))
				source.SourceText.ToString().Dump(source.HintName);

		Report(generatorDiagnostics, outputCompilation);

		using var ms = new MemoryStream();
		var emit = outputCompilation.Emit(ms);
		if (!emit.Success)
			throw new InvalidOperationException("Mapper 编译失败：\n" + string.Join("\n", emit.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error)));

		ms.Position = 0;
		// 加载进查询程序集所在 ALC，保证生成代码里的类型与脚本里的是同一类型
		return AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly())!.LoadFromStream(ms);
	}

	static (CSharpGeneratorDriver, Compilation, IEnumerable<Diagnostic>) RunGenerators(SyntaxTree[] trees, OutputKind outputKind)
	{
		var queryAssembly = Assembly.GetExecutingAssembly();

		// 引用：当前进程里所有落盘的程序集（含查询程序集本身、LINQPad.Runtime、BCL）。
		// Riok.Mapperly.Abstractions 是延迟加载的，脚本没用到它时还不在 AppDomain 里，要显式加上。
		var references = AppDomain.CurrentDomain.GetAssemblies()
			.Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location) && File.Exists(a.Location))
			.Select(a => a.Location)
			.Append(typeof(MapperAttribute).Assembly.Location)
			.Append(queryAssembly.Location)
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.Select(p => MetadataReference.CreateFromFile(p))
			.ToList();

		var compilation = CSharpCompilation.Create(
			"MapperlyGenerated_" + Guid.NewGuid().ToString("N"),
			trees,
			references,
			new CSharpCompilationOptions(outputKind, nullableContextOptions: NullableContextOptions.Enable));

		// 加载与当前 Roslyn 版本匹配的 Riok.Mapperly.dll（生成器），必须加载进 Roslyn 所在的 ALC
		var generatorAssembly = AssemblyLoadContext.GetLoadContext(typeof(CSharpCompilation).Assembly)!
			.LoadFromAssemblyPath(FindGeneratorDll());
		var generators = generatorAssembly.GetTypes()
			.Where(t => !t.IsAbstract && typeof(IIncrementalGenerator).IsAssignableFrom(t))
			.Select(t => ((IIncrementalGenerator)Activator.CreateInstance(t)!).AsSourceGenerator())
			.ToArray();

		// 生成器输出的语法树要与输入用同一语言版本，否则 CSharpCompilation 报“不一致的语言版本”
		var driver = CSharpGeneratorDriver.Create(generators, parseOptions: (CSharpParseOptions)trees[0].Options);
		driver = (CSharpGeneratorDriver)driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);
		return (driver, outputCompilation, diagnostics);
	}

	// Dump 警告/错误，返回 Error 级诊断的 Id（真实 dotnet build 下这些会让编译失败）
	static List<string> Report(IEnumerable<Diagnostic> generatorDiagnostics, Compilation outputCompilation, string title = "Diagnostics")
	{
		var all = generatorDiagnostics.Concat(outputCompilation.GetDiagnostics()).ToList();
		var rows = all
			.Where(d => d.Severity >= DiagnosticSeverity.Warning)
			.Select(d => new
			{
				d.Id,
				Severity = d.Severity.ToString(),
				Mapper = MapperOf(d),
				Line = d.Location.GetLineSpan().StartLinePosition.Line + 1,
				Message = d.GetMessage(),
			})
			.Distinct()
			.ToList();
		if (rows.Count > 0)
			rows.Dump(title);

		return all.Where(d => d.Severity == DiagnosticSeverity.Error).Select(d => d.Id).Distinct().ToList();
	}

	// 诊断所在的 Mapper 类名，一个脚本里有多个 Mapper 时方便区分
	static string MapperOf(Diagnostic d)
	{
		var tree = d.Location.SourceTree;
		if (tree == null) return "";
		return tree.GetRoot().FindNode(d.Location.SourceSpan)
			.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().FirstOrDefault()?.Identifier.Text ?? "";
	}

	// .linq = XML 头部（<Query ...>…</Query> 或 <Query ... />）+ 正文
	static (XElement Header, string Body) SplitQuery(string text)
	{
		var m = Regex.Match(text, @"^\s*<Query\b.*?(?:/>|</Query>)", RegexOptions.Singleline);
		if (!m.Success)
			throw new FormatException("不是合法的 .linq 文件：缺少 <Query> 头部");
		// 头部换成等量空行，诊断里的行号才能和 .linq 文件对上
		return (XElement.Parse(m.Value), new string('\n', m.Value.Count(c => c == '\n')) + text.Substring(m.Length));
	}

	static string FindGeneratorDll()
	{
		// 从已加载的 Riok.Mapperly.Abstractions.dll 向上找包根目录（含 analyzers 文件夹）
		var dir = Path.GetDirectoryName(typeof(MapperAttribute).Assembly.Location);
		while (dir != null && !Directory.Exists(Path.Combine(dir, "analyzers")))
			dir = Path.GetDirectoryName(dir);

		if (dir == null)
		{
			var version = typeof(MapperAttribute).Assembly.GetName().Version!;
			dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
				".nuget", "packages", "riok.mapperly", $"{version.Major}.{version.Minor}.{version.Build}");
		}

		var roslyn = typeof(CSharpCompilation).Assembly.GetName().Version!;
		var candidate = Directory.GetDirectories(Path.Combine(dir, "analyzers"), "roslyn*")
			.Select(d => new { Path = d, Version = Version.Parse(Path.GetFileName(d).Substring("roslyn".Length)) })
			.Where(x => x.Version <= roslyn)
			.OrderByDescending(x => x.Version)
			.FirstOrDefault()
			?? throw new FileNotFoundException($"在 {dir} 下没有找到适配 Roslyn {roslyn} 的 Mapperly 生成器");

		return Path.Combine(candidate.Path, "dotnet", "cs", "Riok.Mapperly.dll");
	}
}

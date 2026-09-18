<Query Kind="Statements">
  <NuGetReference Version="4.3.1">Riok.Mapperly</NuGetReference>
  <NuGetReference Version="4.14.0">Microsoft.CodeAnalysis.CSharp</NuGetReference>
</Query>

#load "MapperlyRunner.linq"

// 扫描本目录（含子目录）下所有声明了 [Mapper] 的可执行 .linq，逐个跑 Mapperly 生成器，
// 在各自旁边写出同名的 .g.linq（脚本通过 #load 引入它）。
// 任何脚本的 Mapper 有改动后运行一次本脚本即可，再回到该脚本正常 F5。
var root = Path.GetDirectoryName(Util.CurrentQueryPath)!;

MapperlyRunner.GenerateAll(root).Dump("生成结果");

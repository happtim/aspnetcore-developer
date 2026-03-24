<Query Kind="Statements">
  <NuGetReference Version="3.29.3">Google.Protobuf</NuGetReference>
  <Namespace>Google.Protobuf</Namespace>
  <Namespace>Google.Protobuf.Collections</Namespace>
  <Namespace>Google.Protobuf.WellKnownTypes</Namespace>
  <Namespace>Rbk.Protocol</Namespace>
  <Namespace>Google.Protobuf.Reflection</Namespace>
</Query>

#load ".\MessageMap.cs"

// 获取当前脚本所在目录，读取 default.smap 文件
var dir = Path.GetDirectoryName(Util.CurrentQueryPath);
var smapPath = Path.Combine(dir, "default.smap");

var jsonText = File.ReadAllText(smapPath);

// 使用 Google.Protobuf 的 JsonParser 将 JSON 反序列化为 Message_Map
var settings = new JsonParser.Settings(
    recursionLimit: JsonParser.Settings.Default.RecursionLimit,
    typeRegistry: TypeRegistry.Empty);

var parser = new JsonParser(settings);
var messageMap = parser.Parse<Message_Map>(jsonText);

// 输出结果
messageMap.Dump("Message_Map");

// 输出基本信息
messageMap.Header.Dump("Header");
$"AdvancedPoints count: {messageMap.AdvancedPointList.Count}".Dump();
$"AdvancedLines count: {messageMap.AdvancedLineList.Count}".Dump();
$"AdvancedAreas count: {messageMap.AdvancedAreaList.Count}".Dump();
$"NormalPosList count: {messageMap.NormalPosList.Count}".Dump();

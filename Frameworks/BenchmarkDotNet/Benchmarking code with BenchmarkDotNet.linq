<Query Kind="Program">
  <NuGetReference>BenchmarkDotNet</NuGetReference>
  <Namespace>BenchmarkDotNet.Attributes</Namespace>
  <Namespace>BenchmarkDotNet.Configs</Namespace>
  <Namespace>BenchmarkDotNet.Running</Namespace>
  <Namespace>System.Security.Cryptography</Namespace>
</Query>

// For industrial-grade benchmarking, add a NuGet reference to BenchmarkDotNet, which has
// special support for LINQPad. The following query compares the hashing performance of
// MD5 vs SHA256. Read more about BenchmarkDotNet here:
//
// https://github.com/dotnet/BenchmarkDotNet/blob/master/README.md

#LINQPad optimize+     // Enable compiler optimizations

void Main()
{
	Util.AutoScrollResults = true;
	BenchmarkRunner.Run<Md5VsSha256>();
}

[ShortRunJob]
public class Md5VsSha256
{
	private const int N = 10000;
	private readonly byte[] data;

	private readonly SHA256 sha256 = SHA256.Create();
	private readonly MD5 md5 = MD5.Create();

	public Md5VsSha256()
	{
		data = new byte [N];
		new Random (42).NextBytes (data);
	}

	[Benchmark]
	public byte[] Sha256() => sha256.ComputeHash (data);

	[Benchmark]
	public byte[] Md5() => md5.ComputeHash (data);
}
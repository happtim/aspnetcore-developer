<Query Kind="Statements">
  <NuGetReference>CavemanTcp</NuGetReference>
  <NuGetReference Version="7.0.0">System.IO.Pipelines</NuGetReference>
  <Namespace>CavemanTcp</Namespace>
  <Namespace>System.IO.Pipelines</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Buffers</Namespace>
  <Namespace>System.Net.Sockets</Namespace>
</Query>


// Instantiate
TcpClient client = new TcpClient();
await client.ConnectAsync("127.0.0.1",6000);

Console.WriteLine("Connected to server.");

Task.Run( async () => await ProcessLinesAsync(client));

var stream = client.GetStream();
byte[] data = Encoding.ASCII.GetBytes("echo" + '\n');
await stream.WriteAsync(data, 0, data.Length);

Console.ReadLine();

async Task ProcessLinesAsync(TcpClient client)
{
	Stream stream = client.GetStream();
	var reader = PipeReader.Create(stream);

	while (true)
	{
		System.IO.Pipelines.ReadResult result = await reader.ReadAsync();
		ReadOnlySequence<byte> buffer = result.Buffer;

		while (TryReadLine(ref buffer,  out ReadOnlySequence<byte> line))
		{
			// Process the line.
			ProcessLine(line);
		}

		reader.AdvanceTo(buffer.Start, buffer.End);

		if (result.IsCompleted)
		{
			break;
		}
	}

}


bool TryReadLine(ref ReadOnlySequence<byte> buffer, out ReadOnlySequence<byte> line)
{
	// Look for a EOL in the buffer.
	SequencePosition? position = buffer.PositionOf((byte)'\n');

	if (position == null)
	{
		line = default;
		return false;
	}

	// Skip the line + the \n.
	line = buffer.Slice(0, position.Value);
	buffer = buffer.Slice(buffer.GetPosition(1, position.Value));
	return true;
}


void ProcessLine(in ReadOnlySequence<byte> buffer)
{
	foreach (var segment in buffer)
	{

		Console.Write(Encoding.UTF8.GetString(segment.ToArray()));

	}
	
	Console.WriteLine();
}

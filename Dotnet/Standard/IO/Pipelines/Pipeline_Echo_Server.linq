<Query Kind="Statements">
  <NuGetReference>CavemanTcp</NuGetReference>
  <NuGetReference Version="7.0.0">System.IO.Pipelines</NuGetReference>
  <Namespace>CavemanTcp</Namespace>
  <Namespace>System.IO.Pipelines</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Buffers</Namespace>
</Query>


// Instantiate
CavemanTcpServer server = new CavemanTcpServer("127.0.0.1", 6000, false, null, null);

// Set callbacks
server.Events.ClientConnected += (s, e) =>
{
	  Console.WriteLine("Client " + e.Client.ToString() + " connected to server");
	  Task.Run (async () => await ProcessLinesAsync(e.Client));
};

server.Events.ClientDisconnected += (s, e) =>
{
	Console.WriteLine("Client " + e.Client.ToString() + " disconnected from server");
};

// Start server
server.Start();

Console.ReadLine();

async Task ProcessLinesAsync(ClientMetadata client)
{
	Stream stream = server.GetStream(client.Guid);
	var reader = PipeReader.Create(stream);

	while (true)
	{
		if(!server.IsConnected(client.Guid))
		{
			return;
		}
		
		System.IO.Pipelines.ReadResult result = await reader.ReadAsync();
		ReadOnlySequence<byte> buffer = result.Buffer;

		while (TryReadLine(ref buffer,  out ReadOnlySequence<byte> line))
		{
			// Process the line.
			ProcessLine(line, client.Guid);
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


void ProcessLine(in ReadOnlySequence<byte> buffer,Guid clientId)
{
	foreach (var segment in buffer)
	{
		string recieve = "Server Recieved:" + Encoding.UTF8.GetString(segment.ToArray());
		server.Send(clientId,recieve + '\n');
		Console.Write(recieve);
	}
	
	Console.WriteLine();
}

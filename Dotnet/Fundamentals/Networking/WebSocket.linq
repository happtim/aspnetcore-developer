<Query Kind="Statements">
  <Namespace>System.Net.WebSockets</Namespace>
</Query>

Uri uri = new("ws://corefx-net-http11.azurewebsites.net/WebSocket/EchoWebSocket.ashx");

using ClientWebSocket ws = new();
await ws.ConnectAsync(uri, default);

var bytes = new byte[1024];
var result = await ws.ReceiveAsync(bytes, default);
string res = Encoding.UTF8.GetString(bytes, 0, result.Count);

await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closed", default);
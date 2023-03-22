<Query Kind="Statements">
  <NuGetReference>DotNetty.Buffers</NuGetReference>
  <Namespace>DotNetty.Buffers</Namespace>
</Query>



IByteBuffer buffer = Unpooled.Buffer();
buffer.WriteByte(2);
buffer.WriteByte(3);

buffer.WriterIndex.Dump("WriterIndex");
buffer.ReaderIndex.Dump("ReaderIndex");
buffer.ReadableBytes.Dump("ReadableBytes");
buffer.WritableBytes.Dump("WritableBytes");
 
var int2 = buffer.ReadByte();
 int2.Dump();

buffer.WriterIndex.Dump("WriterIndex");
buffer.ReaderIndex.Dump("ReaderIndex");
buffer.ReadableBytes.Dump("ReadableBytes");
buffer.WritableBytes.Dump("WritableBytes");

buffer.Dump();
  
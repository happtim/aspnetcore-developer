<Query Kind="Statements">
  <NuGetReference Version="0.7.5">DotNetty.Buffers</NuGetReference>
  <Namespace>DotNetty.Buffers</Namespace>
</Query>


//Unpooled 静态类 用来分配 Unpooled类型的内存。
//Unpooled 默认使用 UnpooledByteBufferAllocator 类进行分配内存。
//Unpooled 使用 UnpooledByteBufferAllocator 可以分配 Buffer （UnpooledHeapByteBuffer） 和  DirectBuffer （UnpooledUnsafeDirectByteBuffer）


IByteBuffer buffer = Unpooled.Buffer();
IByteBuffer directBuffer = Unpooled.DirectBuffer();
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
  
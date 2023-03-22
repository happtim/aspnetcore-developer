<Query Kind="Statements" />


//IByteBuffer
//三个重要指针
/* https://github.com/netty/netty/blob/4.1/buffer/src/main/java/io/netty/buffer/ByteBuf.java
*	   +-------------------+------------------+------------------+
*      | discardable bytes | readable bytes   | writable bytes   |
*      |                   | (CONTENT)        |                  |
*	   +-------------------+------------------+------------------+
*      |                   |                  |                  |
*	   0		<= 		readerIndex   <= writerIndex 	<= 	  capacity     <= maxCapacity
*
*      0 - readIndex 为无效数据
*      readIndex ->  writerIndex 待读取数据
* 	   writerIndex -> capacity   可写数据
*/

	
	//三个方法
	//read 读取对应长度数据 并且移动读指针
	//warite 写入对应长度数据 并且移动写指针
	//set  在某个位置写入数据 并且移动写指针

//提供IByteBuffer骨架类。
//AbstractByteBuffer

//Pooled （池化）和 Unpooled （非池化）
//Safe 和 Unsafe （非安全）
//Buffer （byte[] array） 和 Direct （java.nio.ByteBuffer） DotNetty 没有对Buffer/Direct 进行区分。内部都使用Buffer

//abstract class PooledByteBuffer<T> : AbstractReferenceCountedByteBuffer

	//池化堆
	//class PooledHeapByteBuffer : PooledByteBuffer<byte[]>

	//池化非安全ByteBuffer
	//class PooledUnsafeDirectByteBuffer : PooledByteBuffer<byte[]>


//非池化堆
//class UnpooledHeapByteBuffer : AbstractReferenceCountedByteBuffer

//非池化非安全ByteBuffer
//class UnpooledUnsafeDirectByteBuffer : AbstractReferenceCountedByteBuffer


int i = 5;
// Unsafe method: uses address-of operator (&).
SquarePtrParam(&i);

i.Dump();

unsafe static void SquarePtrParam(int* p)
{
	*p *= *p;
}


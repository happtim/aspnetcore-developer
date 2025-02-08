<Query Kind="Statements">
  <NuGetReference Version="0.7.5">DotNetty.Handlers</NuGetReference>
  <NuGetReference Version="0.7.5">DotNetty.Transport</NuGetReference>
  <Namespace>DotNetty.Codecs</Namespace>
  <Namespace>DotNetty.Transport.Channels</Namespace>
  <Namespace>DotNetty.Buffers</Namespace>
  <Namespace>System.Numerics</Namespace>
</Query>

public class NumberEncoder : MessageToMessageEncoder<System.Numerics.BigInteger>
{
	protected override void Encode(IChannelHandlerContext context, System.Numerics.BigInteger message, List<object> output)
	{
		IByteBuffer buffer = context.Allocator.Buffer();

		//https://msdn.microsoft.com/en-us/library/system.numerics.biginteger.tobytearray(v=vs.110).aspx
		//BigInteger.ToByteArray() return a Little-Endian bytes
		//IByteBuffer is Big-Endian by default
		byte[] data = message.ToByteArray();
		buffer.WriteByte((byte)'F');
		buffer.WriteInt(data.Length);
		buffer.WriteBytes(data);
		output.Add(buffer);
	}
}

public class BigIntegerDecoder : ByteToMessageDecoder
{
	protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
	{
		if (input.ReadableBytes < 5)
		{
			return;
		}
		input.MarkReaderIndex();

		int magicNumber = input.ReadByte();
		if (magicNumber != 'F')
		{
			input.ResetReaderIndex();
			throw new Exception("Invalid magic number: " + magicNumber);
		}
		int dataLength = input.ReadInt();
		if (input.ReadableBytes < dataLength)
		{
			input.ResetReaderIndex();
			return;
		}
		var decoded = new byte[dataLength];
		input.ReadBytes(decoded);

		output.Add(new BigInteger(decoded));
	}
}
<Query Kind="Expression" />

//https://github.com/Azure/DotNetty/tree/dev/src/DotNetty.Codecs

//解码器
//主要就是字节数组转换为消息对象
//解码器具体的实现，用的比较多的是(更多是为了解决TCP底层的粘包和拆包问题)

//用于将字节转为消息，需要检查缓冲区是否有足够的字节
//abstract class ByteToMessageDecoder : ChannelHandlerAdapter

//不需要检查缓冲区是否有足够的字节，但是ReplayingDecoder速度略慢于ByteToMessageDecoder，不是所有的ByteBuf都支持。
//abstract class ReplayingDecoder<TState> : ByteToMessageDecoder

//按照行分隔字节
//class LineBasedFrameDecoder : ByteToMessageDecoder

//按照分隔符分隔字节
//MaxLength：表示一行最大的长度，如果超过这个长度依然没有检测自定义分隔符，将会抛出TooLongFrameException
//FailFast：如果为true，则超出maxLength后立即抛出TooLongFrameException，不进行继续解码.如果为False，则等到完整的消息被解码后，再抛出TooLongFrameException异常
//StripDelimiter：解码后的消息是否去除掉分隔符
//Delimiters：分隔符，ByteBuf类型
//class DelimiterBasedFrameDecoder : ByteToMessageDecoder

//按照长度字段去分隔字节
//MaxFrameLength：数据包的最大长度
//LengthFieldOffset：长度字段的偏移位，长度字段开始的地方，意思是跳过指定长度个字节之后的才是消息体字段
//LengthFieldLength：长度字段占的字节数, 帧数据长度的字段本身的长度
//LengthAdjustment：一般 Header +Body，添加到长度字段的补偿值,如果为负数，开发人员认为这个 Header的长度字段是整个消息包的长度，则Netty应该减去对应的数字
//InitialBytesToStrip：从解码帧中第一次去除的字节数, 获取完一个完整的数据包之后，忽略前面的指定位数的长度字节，应用解码器拿到的就是不带长度域的数据包
//class LengthFieldBasedFrameDecoder : ByteToMessageDecoder


//用于从一种消息解码为另外一种消息（例如POJO到POJO）
//abstract class MessageToMessageDecoder<T> : ChannelHandlerAdapter

//将字节解码为string
//class StringDecoder : MessageToMessageDecoder<IByteBuffer>


//编码器
//消息对象转换为字节数组

//将消息转为字节数组,调用write方法，会先判断当前编码器是否支持需要发送的消息类型，如果不支持，则透传；
//abstract class MessageToByteEncoder<T> : ChannelHandlerAdapter

//用于从一种消息编码为另外一种消息
//abstract class MessageToMessageEncoder<T> : ChannelHandlerAdapter

//给字节增加长度信息
//class LengthFieldPrepender : MessageToMessageEncoder<IByteBuffer>

//将字符串转化为字节
//class StringEncoder : MessageToMessageEncoder<string>


//编解码
//组合解码器和编码器，以此提供对于字节和消息都相同的操作
//优点：成对出现，编解码都是在一个类里面完成
//缺点：耦合在一起，拓展性不佳

//abstract class MessageToMessageCodec<TInbound, TOutbound> : ChannelDuplexHandler

	//http消息的编解码
	//abstract class HttpContentEncoder : MessageToMessageCodec<IHttpRequest, IHttpObject>
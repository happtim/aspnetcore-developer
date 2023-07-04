<Query Kind="Statements">
  <NuGetReference Version="0.7.5">DotNetty.Buffers</NuGetReference>
  <NuGetReference Version="0.7.5">DotNetty.Codecs</NuGetReference>
  <NuGetReference Version="0.7.5">DotNetty.Handlers</NuGetReference>
  <Namespace>DotNetty.Buffers</Namespace>
  <Namespace>DotNetty.Handlers.Timeout</Namespace>
</Query>

//单例模式
//DotNetty.Handlers/Timeout/ReadTimeoutException.cs
var exception =  ReadTimeoutException.Instance;

//DotNetty.Codecs.Mqtt.MqttEncoder

//装饰
//ByteBuffer有多个子类，所以不能给所有子类都继承一个Unreleasable。所以需要使用装饰设计模式修改类的实现。
//UnreleasableByteBuffer
//SimpleLeakAwareByteBuffer

//组合
//CompositeByteBuffer
//WrappedCompositeByteBuffer
//

//迭代器

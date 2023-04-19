<Query Kind="Statements" />



var pipeline = new DefaultChannelPipeline();
pipeline.AddLast(new My1ChannelHandlerAdapter());
pipeline.AddLast(new My2ChannelHandlerAdapter());
pipeline.FireChannelRegistered();

pipeline.FireChannelRead( new UTF8Encoding().GetBytes( "*你好Tim*") );


public class My1ChannelHandlerAdapter : ChannelHandlerAdapter
{
	public override void ChannelRegistered(IChannelHandlerContext context)
	{
		Console.WriteLine("注册成功1");
		base.ChannelRegistered(context);
	}

	public override void ChannelRead(IChannelHandlerContext context, object message)
	{
		message.Dump();
		var hello =  new UTF8Encoding().GetString( ( byte[] )message);
		base.ChannelRead(context, hello);
	}
}

public class My2ChannelHandlerAdapter : ChannelHandlerAdapter
{
	public override void ChannelRegistered(IChannelHandlerContext context)
	{
		Console.WriteLine("注册成功2");
		base.ChannelRegistered(context);
	}

	public override void ChannelRead(IChannelHandlerContext context, object message)
	{
		message.Dump();
		var hello = (string)message;
		hello = hello.Trim('*');
		hello.Dump();
		base.ChannelRead(context, message);
	}
}


public interface IChannelPipeline : IEnumerable<IChannelHandler>, IEnumerable
{
	IChannelPipeline AddLast(params IChannelHandler[] handlers);
	IChannelPipeline FireChannelRegistered();
	IChannelPipeline FireChannelRead(object msg);
}

public class DefaultChannelPipeline : IChannelPipeline
{
	public DefaultChannelPipeline()
	{
		tail = new TailContext(this);
		head = new HeadContext(this);
		head.Next = tail;
		tail.Prev = head;
	}

	private readonly AbstractChannelHandlerContext head;

	private readonly AbstractChannelHandlerContext tail;

	public IChannelPipeline FireChannelRegistered()
	{
		AbstractChannelHandlerContext.InvokeChannelRegistered(head);
		return this;
	}

	public IChannelPipeline FireChannelRead(object msg)
	{
		head.FireChannelRead(msg);
		return this;
	}

	public IEnumerator<IChannelHandler> GetEnumerator()
	{
		for (AbstractChannelHandlerContext current = head; current != null; current = current.Next)
		{
			yield return current.Handler;
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable<IChannelHandler>)this).GetEnumerator();
	}

	public IChannelPipeline AddLast(params IChannelHandler[] handlers)
	{
		foreach (IChannelHandler handler in handlers)
		{
			AbstractChannelHandlerContext abstractChannelHandlerContext;
			lock (this)
			{
				abstractChannelHandlerContext = NewContext(handler);
				AddLast0(abstractChannelHandlerContext);
			}
		}
		return this;
	}

	private void AddLast0(AbstractChannelHandlerContext newCtx)
	{
		AbstractChannelHandlerContext abstractChannelHandlerContext = (newCtx.Prev = tail.Prev);
		newCtx.Next = tail;
		abstractChannelHandlerContext.Next = newCtx;
		tail.Prev = newCtx;
	}

	private AbstractChannelHandlerContext NewContext( IChannelHandler handler)
	{
		return new DefaultChannelHandlerContext(this, handler);
	}


	private sealed class TailContext : AbstractChannelHandlerContext, IChannelHandler
	{
		public override IChannelHandler Handler => this;

		public TailContext(DefaultChannelPipeline pipeline)
			: base(pipeline)
		{

		}

		public void ChannelRegistered(IChannelHandlerContext context)
		{ }

		public void ChannelRead(IChannelHandlerContext context, object message)
		{ }
	}

	private sealed class HeadContext : AbstractChannelHandlerContext, IChannelHandler
	{
		public override IChannelHandler Handler =>this;
		private bool firstRegistration = true;
		
		public HeadContext(DefaultChannelPipeline pipeline)
			:base(pipeline)
		{
			
		}

		public void ChannelRead(IChannelHandlerContext context, object message)
		{
			context.FireChannelRead(message);
		}

		public void ChannelRegistered(IChannelHandlerContext context)
		{
			if (firstRegistration)
			{
				firstRegistration = false;
				//pipeline.CallHandlerAddedForAllHandlers();
			}
			context.FireChannelRegistered();
		}
	}
}


public class ChannelHandlerAdapter : IChannelHandler
{
	public virtual void ChannelRegistered(IChannelHandlerContext context)
	{
		context.FireChannelRegistered();
	}

	public virtual void ChannelRead(IChannelHandlerContext context, object message)
	{
		context.FireChannelRead(message);
	}
}

public interface IChannelHandler
{
	void ChannelRegistered(IChannelHandlerContext context);
	void ChannelRead(IChannelHandlerContext context, object message);
}

public class DefaultChannelHandlerContext : AbstractChannelHandlerContext
{
	public override IChannelHandler Handler {get;}
	
	public DefaultChannelHandlerContext(DefaultChannelPipeline pipeline,IChannelHandler handler)
		:base(pipeline)
	{
		Handler = handler;
	}
}

public abstract class AbstractChannelHandlerContext : IChannelHandlerContext
{
	public abstract IChannelHandler Handler { get; }
	internal readonly DefaultChannelPipeline pipeline;
	internal volatile AbstractChannelHandlerContext Next;
	internal volatile AbstractChannelHandlerContext Prev;
	
	protected  AbstractChannelHandlerContext(DefaultChannelPipeline pipeline)
	{
		this.pipeline = pipeline;
	}
	
	public IChannelHandlerContext FireChannelRegistered()
	{
		InvokeChannelRegistered(FindContextInbound());
		return this;
	}

	internal static void InvokeChannelRegistered(AbstractChannelHandlerContext next) 
	{
		next.InvokeChannelRegistered();
	}
	
	private void InvokeChannelRegistered()
	{
		Handler.ChannelRegistered(this);
	}

	public IChannelHandlerContext FireChannelRead(object message)
	{
		Handler.ChannelRead(FindContextInbound(), message);
		return this;
	}

	private AbstractChannelHandlerContext FindContextInbound()
	{
		AbstractChannelHandlerContext abstractChannelHandlerContext = this;
	
		abstractChannelHandlerContext = abstractChannelHandlerContext.Next;

		return abstractChannelHandlerContext;
	}
}

public interface IChannelHandlerContext
{
	 IChannelHandlerContext FireChannelRegistered();
	 IChannelHandlerContext FireChannelRead(object message);
}
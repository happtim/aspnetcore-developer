<Query Kind="Statements" />

//DotNetty PipeLine
//AspNetCore PipeLine
//Elas PipeLine

// 创建管道
Pipeline<string> pipeline = new Pipeline<string>();

// 添加处理步骤到管道中
pipeline.AddStep(new UpperCaseStep())
		.AddStep(new ReverseStep());
		
pipeline.Execute("hello world").Dump();


// 定义处理步骤接口
public interface IStep<TInput, TOutput>
{
	TOutput Process(TInput input);
}

// 将字符串转换为大写的处理步骤
public class UpperCaseStep : IStep<string, string>
{
	public string Process(string input)
	{
		return input.ToUpper();
	}
}

// 将字符串反转的处理步骤
public class ReverseStep : IStep<string, string>
{
	public string Process(string input)
	{
		return new string(input.Reverse().ToArray());
	}
}

// 管道类
public class Pipeline<T>
{
	private List<IStep<T, T>> steps;

	public Pipeline()
	{
		steps = new List<IStep<T, T>>();
	}

	// 添加处理步骤到管道中
	public Pipeline<T> AddStep(IStep<T, T> step)
	{
		steps.Add(step);
		return this;
	}

	// 执行管道处理
	public T Execute(T input)
	{

		foreach (IStep<T, T> step in steps)
		{
			input = step.Process(input);
		}

		return input;
	}
}
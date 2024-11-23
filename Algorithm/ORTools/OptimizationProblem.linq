<Query Kind="Expression" />

//什么是优化问题？
//优化的目标是从大量可能的解决方案中找到问题的最佳解决方案。（有时，您会对找到任何可行的解决方案感到满意；OR 工具也可以做到这一点。）

//下面是一个典型的优化问题。假设一家货运公司使用一支卡车将包裹交付给客户。每天，该公司必须将包裹分配给卡车，然后为每辆卡车选择配送路线。
//每次可能的包裹和路线分配都会产生成本，具体费用取决于卡车的总行程距离，可能还涉及其他因素。问题是选择费用最低的套餐和路由的分配。

//与所有优化问题一样，此类问题具有以下元素：

	//目标：要优化的数量。 上面的示例中的目标是最大限度地减少费用。 如需设置优化问题，您需要定义一个函数，用于计算任何可能的解决方案的目标值。
	//这称为目标函数。在前面的示例中，目标函数将计算任何软件包和路线分配的总费用。

	//最佳解决方案是：指目标函数的值是最佳解决方案。（“最佳”可以是最大值，也可以是最小值。）

	//限制条件： 根据问题的具体要求对可能的解决方案集的限制。 例如，如果货运公司无法将超过给定重量的包裹分配给卡车，这会对解决方案施加限制。

	//可行解决方案：满足针对问题的所有给定约束条件，而不一定是最优解决方案。

//解决优化问题的第一步是确定目标和限制。
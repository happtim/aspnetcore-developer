<Query Kind="Expression" />


//DbContext 的生命周期

//DbContext 的生命周期从实例创建开始，到实例被销毁结束。
//一个 DbContext 实例被设计用于一个单元操作。这意味着 DbContext 实例的生命周期通常非常短。

//工作单元：工作单元跟踪您在业务交易期间对数据库可能产生影响的所有操作。当您完成时，它会计算出需要对数据库进行的所有更改。

//使用Entity Framework Core（EF Core）时，典型的工作单元包括：

//* 创建一个DbContext实例
//* 上下文中的实体实例跟踪。实体通过跟踪变得可追踪：
// 1 从查询中返回（Query）
// 2 被添加或附加到上下文中（Add）
//* 根据需要对被跟踪的实体进行更改以实施业务规则
//* 调用SaveChanges或SaveChangesAsync。EF Core检测到所做的更改并将其写入数据库。
//* DbContext实例已被释放

//注意
//DbContext不是线程安全的。不要在线程之间共享上下文。确保在继续使用上下文实例之前等待所有异步调用。
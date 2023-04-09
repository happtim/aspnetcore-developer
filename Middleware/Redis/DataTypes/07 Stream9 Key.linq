<Query Kind="Expression" />

// - +：使用在XRange 代表最小的id  和最大的id
//$: 代表一个时间点的id，后续插入的id都比他大。
// + 和 $ 区别
	// + :代表 stream中最大的id
	// $ :代表 stream中当时最大的id
	
// >: 使用在 XReadGroup 代表没有投递给其他的消费者的id
// *：使用在 XAdd中 代表自动生成id
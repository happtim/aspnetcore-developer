<Query Kind="Expression" />

// 安装依赖
// 	1. Erlang
// 
// 运行
// 	安装之后就注册服务可以自动运行。
// 
// 开启UI插件
// rabbitmq-plugins list 查看所有插件。
// rabbitmq-plugins enable rabbitmq_management
//
// 端口
// 5672  clients without and with TLS
// 15672 rabbitmqadmin UI 
// 	http://127.0.0.1:15672/#/
//  guest guest


// 术语
//Producer：生产者，就是投递消息的一方。
//Consumer：消费者，就是接收消息的一方。
//Broker：消息中间件的服务节点。
//Queue：队列，是RabbitMQ的内部对象，用于存储消息。
//	多个消费者可以订阅同一个队列，这时队列中的消息会被平均分摊（Round-Robin，即轮询）给多个消费者进行处理，
// 而不是每个消费者都收到所有的消息并处理，RabbitMQ不支持队列层面的广播消费。/
//Exchange：生产者将消息发送到Exchange（交换器，通常也可以用大写的“X”来表示），由交换器将消息路由到一个或者多个队列中。
//	 四种类型：direct, topic, headers and fanout.
//Binding：绑定。RabbitMQ中通过绑定将交换器与队列关联起来，在绑定的时候一般会指定一个绑定键（BindingKey），这样RabbitMQ就知道如何正确地将消息路由到队列了




﻿using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory();

factory.Uri = new Uri("amqp://guest:guest@localhost:5672");

factory.ClientProvidedName = "Rabbit Receiver 1";

var connection = factory.CreateConnection();

var channel = connection.CreateModel();

var exchangeName = "Demo Exchange";
var routingKey = "demo-routing-key";
var queueName = "DemoQueue";

channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
channel.QueueDeclare(queueName, false, false, false, null);
channel.QueueBind(queueName, exchangeName, routingKey, null);
channel.BasicQos(0, 1, false);

var consumer = new EventingBasicConsumer(channel);

consumer.Received += (sender, args) =>
{
    Console.WriteLine(sender!.ToString());
    try
    {
        Task.Delay(TimeSpan.FromSeconds(5)).Wait();

        var body = args.Body.ToArray();

        var message = Encoding.UTF8.GetString(body);

        Console.WriteLine($"Message Received from Queue: {message}");

        channel.BasicAck(args.DeliveryTag, false);
    }
    catch 
    { 
    }
    
};


string consumerTag = channel.BasicConsume(queueName, false, consumer);

Console.ReadLine();

channel.BasicCancel(consumerTag);

channel.Close();

connection.Close();
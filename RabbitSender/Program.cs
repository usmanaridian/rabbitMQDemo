using RabbitMQ.Client;
using System.Diagnostics.Metrics;
using System.Text;

var factory = new ConnectionFactory();

factory.Uri = new Uri("amqp://guest:guest@localhost:5672");

factory.ClientProvidedName = "Rabbit Sender App";

var connection = factory.CreateConnection();

var channel = connection.CreateModel();

var exchangeName = "Demo Exchange";
var routingKey = "demo-routing-key";
var queueName = "DemoQueue";

channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
channel.QueueDeclare(queueName, false, false, false, null);
channel.QueueBind(queueName, exchangeName , routingKey, null);

for (int counter = 1; counter <= 60; counter++)
{
    Console.WriteLine($"Sending Message {counter}!");

    byte[] messageBodyBytes = Encoding.UTF8.GetBytes($"Hello RabbitMQ Counter {counter}!");

    channel.BasicPublish(exchangeName, routingKey, null, messageBodyBytes);

    Thread.Sleep(1000);
}

channel.Close();

connection.Close();
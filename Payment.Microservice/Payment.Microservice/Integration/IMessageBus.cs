using Payment.Microservice.Models;

public interface IMessageBus
{
    Task PublishMessage(IntegrationBaseMessageModel message, string topicName);
}
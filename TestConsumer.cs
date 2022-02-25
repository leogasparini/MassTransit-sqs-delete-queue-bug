using MassTransit;

namespace MassTransitTest;

public sealed record MessageValue(string Value);

public sealed class TestConsumer : IConsumer<MessageValue>
{
    public Task Consume(ConsumeContext<MessageValue> context)
    {
        Console.WriteLine("Consumed message value: {0}", context.Message.Value);
        return Task.CompletedTask;
    }
}
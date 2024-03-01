using System;

namespace Messaging;

public class MessageBus
{
    private static MessageBus instance;
    private IMessageBroker messageBroker;

    static MessageBus()
    {
        instance = new MessageBus();
    }
    
    public MessageBus()
    {
        messageBroker = new MessageBroker();
    }
    
    public static Subscription Subscribe<T>(Action<T> handler) where T : class
    {
        return instance.messageBroker.Subscribe(handler);
    }

    public static Subscription Subscribe(Type messageType, Action<object> handler)
    {
        return instance.messageBroker.Subscribe(messageType, handler);
    }

    public static void Unsubscribe(Subscription subscription)
    {
        instance.messageBroker.Unsubscribe(subscription);
    }

    public static void Send<T>(T message)
    {
        instance.messageBroker.Send(message);
    }

    public static void Send(Type messageType, object message)
    {
        instance.messageBroker.Send(messageType, message);
    }
}
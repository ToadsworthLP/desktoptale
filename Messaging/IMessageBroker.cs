using System;

namespace Messaging;

public interface IMessageBroker
{
    public Subscription Subscribe<T>(Action<T> handler) where T : class;
    public Subscription Subscribe(Type messageType, Action<object> handler);
    public void Unsubscribe(Subscription subscription);
    public void Send<T>(T message);
    public void Send(Type messageType, object message);
}
using System;

namespace Desktoptale.Messaging
{
    public interface IMessageBroker
    {
        Subscription Subscribe<T>(Action<T> handler) where T : class;
        Subscription Subscribe(Type messageType, Action<object> handler);
        void Unsubscribe(Subscription subscription);
        void Send<T>(T message);
        void Send(Type messageType, object message);
    }
}
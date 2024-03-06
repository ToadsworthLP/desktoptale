using System;
using System.Collections.Generic;

namespace Desktoptale.Messaging
{
    public class MessageBroker : IMessageBroker
    {
        private IDictionary<Type, ISet<Action<object>>> handlers;
        private IList<(Type type, Action<object> handler)> invalidTargets;

        public MessageBroker()
    {
        handlers = new Dictionary<Type, ISet<Action<object>>>();
        invalidTargets = new List<(Type type, Action<object> handler)>();
    }

        public Subscription Subscribe<T>(Action<T> handler) where T : class
    {
        return Subscribe(typeof(T), msg => handler((T)msg));
    }

        public Subscription Subscribe(Type messageType, Action<object> handler)
    {
        Subscription subscription = new Subscription(messageType, handler);
        
        if (!handlers.ContainsKey(messageType)) handlers.Add(messageType, new HashSet<Action<object>>());
        handlers[messageType].Add(handler);

        return subscription;
    }

        public void Unsubscribe(Subscription subscription)
    {
        if (handlers.ContainsKey(subscription.messageType))
            handlers[subscription.messageType].Remove(subscription.handler);
    }

        public void Send<T>(T message)
    {
        Send(typeof(T), message);
    }

        public void Send(Type messageType, object message)
    {
        if (handlers.TryGetValue(messageType, out ISet<Action<object>> handlerSet))
        {
            foreach (Action<object> handler in handlerSet)
            {
                if (handler.Target != null)
                {
                    handler.Invoke(message);
                }
                else
                {
                    invalidTargets.Add((messageType, handler));
                }
            }

            if (invalidTargets.Count > 0)
            {
                foreach (var target in invalidTargets)
                {
                    if (handlers.ContainsKey(target.type))
                        handlers[target.type].Remove(target.handler);
                }
                
                invalidTargets.Clear();
            }
        }
    }
    }
}
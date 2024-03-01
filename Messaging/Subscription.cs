using System;

namespace Messaging;

public class Subscription
{
    internal Type messageType { get; }
    internal Action<object> handler { get; }
    
    public Subscription(Type messageType, Action<object> handler)
    {
        this.messageType = messageType;
        this.handler = handler;
    }
}
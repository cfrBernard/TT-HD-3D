using System;
using System.Collections.Generic;

public static class GameEventBus
{
    private static readonly Dictionary<Type, Delegate> events = new();

    public static void Subscribe<T>(Action<T> callback)
    {
        if (events.TryGetValue(typeof(T), out var del))
            events[typeof(T)] = Delegate.Combine(del, callback);
        else
            events[typeof(T)] = callback;
    }

    public static void Unsubscribe<T>(Action<T> callback)
    {
        if (events.TryGetValue(typeof(T), out var del))
        {
            var currentDel = Delegate.Remove(del, callback);
            if (currentDel == null) events.Remove(typeof(T));
            else events[typeof(T)] = currentDel;
        }
    }

    public static void Publish<T>(T eventData)
    {
        if (events.TryGetValue(typeof(T), out var del))
        {
            (del as Action<T>)?.Invoke(eventData);
        }
    }
}

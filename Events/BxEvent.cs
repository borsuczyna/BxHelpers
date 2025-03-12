using System;

namespace BxHelpers.Events;

/// <summary>
/// Attribute to mark a method as an event handler for a specific type of event.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class BxEvent : Attribute
{
    public string EventName { get; private set; }
    public Type Type { get; private set; }

    /// <summary>
    /// Adds BxEvent event handler to a method.
    /// </summary>
    /// <param name="type">The type of the event.</param>
    /// <returns></returns>
    public BxEvent(Type type)
    {
        Type = type;
        EventName = type.Name;
    }
}
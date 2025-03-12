using System;
using CitizenFX.Core;
using System.Reflection;
using Models.Events;
using System.Collections.Generic;

namespace BxHelpers.Events;

public class BxEvents : BaseScript
{
    /// <summary>
    /// Stores a history of event hashes to prevent resending the same event.
    /// </summary>
    public static List<string> _hashHistory = new List<string>();

    #region Constructor
    public BxEvents()
    {
        // General event handler
        EventHandlers["triggerEvent"] += new Action<string, string>(OnTriggerEvent);

        #if SERVER
        // Server-side event handler
        EventHandlers["triggerServerEvent"] += new Action<Player, string, string>(OnTriggerServerEvent);
        #endif

        #if CLIENT
        // Client-side event handler
        EventHandlers["triggerClientEvent"] += new Action<string, string>(OnTriggerClientEvent);
        #endif
    }
    #endregion

    #region Hash history
    /// <summary>
    /// Adds a hash to the history, maintaining a max size of 50.
    /// </summary>
    public static void AddHash(string hash)
    {
        if (_hashHistory.Count >= 50)
        {
            _hashHistory.RemoveAt(0);
        }
        _hashHistory.Add(hash);
    }

    /// <summary>
    /// Checks if a hash exists in history.
    /// </summary>
    public static bool HashExists(string hash)
    {
        return _hashHistory.Contains(hash);
    }
    #endregion

    #region Server events
    #if CLIENT
    /// <summary>
    /// Sends a server event with the specified event data.
    /// </summary>
    public static void SendServerEvent(BaseEvent data)
    {
        var dataType = data.GetType().Name;
        var serialized = BxEncoder.Encode(BxSerializer.Serialize(data));
        TriggerServerEvent("triggerServerEvent", dataType, serialized);
    }
    #endif

    #if SERVER
    /// <summary>
    /// Handles incoming server events.
    /// </summary>
    private void OnTriggerServerEvent([FromSource] Player client, string dataType, string serializedData)
    {
        var data = BxSerializer.Deserialize<BaseEvent>(BxEncoder.Decode(serializedData));

        if (!data.Validate())
        {
            Debug.WriteLine($"Invalid data received from client {client.Handle}.");
            return;
        }

        if (HashExists(data.Hash))
        {
            Debug.WriteLine($"Duplicate data received from client {client.Handle}.");
            return;
        }

        AddHash(data.Hash);

        // Process event if a corresponding method exists
        var types = Assembly.GetExecutingAssembly().GetTypes();
        foreach (var type in types)
        {
            var methods = type.GetMethods();
            foreach (var method in methods)
            {
                var attr = (BxEvent)Attribute.GetCustomAttribute(method, typeof(BxEvent));
                if (attr != null && attr.Type.Name == dataType)
                {
                    var deserializedData = BxSerializer.Deserialize(BxEncoder.Decode(serializedData), attr.Type);
                    if (!method.IsStatic)
                    {
                        Debug.WriteLine("Method must be static!!");
                        return;
                    }
                    method.Invoke(this, [client, deserializedData]);
                }
            }
        }
    }
    #endif
    #endregion

    #region Client events
    #if SERVER
    /// <summary>
    /// Sends a client event to a specific player.
    /// </summary>
    public static void SendClientEvent(Player client, BaseEvent data)
    {
        var dataType = data.GetType().Name;
        var serialized = BxEncoder.Encode(BxSerializer.Serialize(data));
        TriggerClientEvent(client, "triggerClientEvent", dataType, serialized);
    }
    #endif

    #if CLIENT
    /// <summary>
    /// Handles incoming client events.
    /// </summary>
    private void OnTriggerClientEvent(string dataType, string serializedData)
    {
        var types = Assembly.GetExecutingAssembly().GetTypes();
        foreach (var type in types)
        {
            var methods = type.GetMethods();
            foreach (var method in methods)
            {
                var attr = (BxEvent)Attribute.GetCustomAttribute(method, typeof(BxEvent));
                if (attr != null && attr.Type.Name == dataType)
                {
                    var deserializedData = BxSerializer.Deserialize(BxEncoder.Decode(serializedData), attr.Type);
                    if (!method.IsStatic)
                    {
                        Debug.WriteLine("Method must be static!!");
                        return;
                    }
                    method.Invoke(this, [deserializedData]);
                }
            }
        }
    }
    #endif
    #endregion

    #region Same side events
    /// <summary>
    /// Sends an event on the same execution side (client or server).
    /// </summary>
    public static void SendEvent(BaseEvent data)
    {
        var dataType = data.GetType().Name;
        var serialized = BxEncoder.Encode(BxSerializer.Serialize(data));
        TriggerEvent("triggerEvent", dataType, serialized);
    }

    /// <summary>
    /// Handles internal event triggering.
    /// </summary>
    private void OnTriggerEvent(string dataType, string serializedData)
    {
        var types = Assembly.GetExecutingAssembly().GetTypes();
        foreach (var type in types)
        {
            var methods = type.GetMethods();
            foreach (var method in methods)
            {
                var attr = (BxEvent)Attribute.GetCustomAttribute(method, typeof(BxEvent));
                if (attr != null && attr.Type.Name == dataType)
                {
                    var deserializedData = BxSerializer.Deserialize(BxEncoder.Decode(serializedData), attr.Type);
                    if (!method.IsStatic)
                    {
                        Debug.WriteLine("Method must be static!!");
                        return;
                    }
                    method.Invoke(this, [deserializedData]);
                }
            }
        }
    }
    #endregion
}
using System;
using System.Collections.Generic;
using CitizenFX.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BxHelpers;

public enum BxSerializedValueType
{
    Entity,
    Player,
}

public class BxSerializedValue
{
    public BxSerializedValueType Type { get; set; }
    public string Value { get; set; }

    public BxSerializedValue(BxSerializedValueType type, string value)
    {
        Type = type;
        Value = value;
    }
}

public class BxJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        var canConvert = typeof(Entity).IsAssignableFrom(objectType) || typeof(Player).IsAssignableFrom(objectType);
        return canConvert;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value is Entity entity)
        {
            var networkEntity = BxNetworkHelper.ToNetworkEntity(entity);
            if (networkEntity == null)
            {
                writer.WriteNull();
                return;
            }

            var serializedValue = new BxSerializedValue(BxSerializedValueType.Entity, networkEntity);
            serializer.Serialize(writer, serializedValue);
        }
        else if (value is Player player)
        {
            var networkEntity = BxNetworkHelper.ToNetworkEntity(player);
            if (networkEntity == null)
            {
                writer.WriteNull();
                return;
            }

            var serializedValue = new BxSerializedValue(BxSerializedValueType.Player, networkEntity);
            serializer.Serialize(writer, serializedValue);
        }
        else
        {
            writer.WriteNull();
        }
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var jObject = JObject.Load(reader);
        var serializedValue = jObject.ToObject<BxSerializedValue>();

        if (serializedValue?.Value != null)
        {
            if (serializedValue.Type == BxSerializedValueType.Player)
            {
                var val = BxNetworkHelper.FromNetworkEntity(serializedValue.Value) as Player;
                return val;
            }
            else if (serializedValue.Type == BxSerializedValueType.Entity)
            {
                return BxNetworkHelper.FromNetworkEntity(serializedValue.Value) as Entity;
            }
        }

        return null;
    }
}

public static class BxSerializer
{
    public static string Serialize(object value)
    {
        var settings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter> { new BxJsonConverter() },
            TypeNameHandling = TypeNameHandling.None,
        };

        return JsonConvert.SerializeObject(value, settings);
    }

    public static T? Deserialize<T>(string value) where T : class
    {
        // first, try deserialize it to BxSerializedValue, if it succeeds, then return it
        try
        {
            var serializedValue = JsonConvert.DeserializeObject<BxSerializedValue>(value);
            if (serializedValue != null)
            {
                if (serializedValue.Type == BxSerializedValueType.Player)
                {
                    var val = BxNetworkHelper.FromNetworkEntity(serializedValue.Value) as T;
                    return val;
                }
                else if (serializedValue.Type == BxSerializedValueType.Entity)
                {
                    return BxNetworkHelper.FromNetworkEntity(serializedValue.Value) as T;
                }
            }
        }
        catch { }

        var settings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter> { new BxJsonConverter() },
            TypeNameHandling = TypeNameHandling.None,
        };

        return JsonConvert.DeserializeObject<T>(value, settings);
    }

    public static object Deserialize(string value, Type type)
    {
        var settings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter> { new BxJsonConverter() },
            TypeNameHandling = TypeNameHandling.Auto,
        };

        return JsonConvert.DeserializeObject(value, type, settings);
    }
}
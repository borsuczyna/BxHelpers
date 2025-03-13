using System.Linq;
using CitizenFX.Core;

namespace BxHelpers;

/// <summary>
/// Helper class for managing network entities and players.
/// </summary>
public class BxNetworkHelper : BaseScript
{
    private static PlayerList PlayersList = null!;

    public BxNetworkHelper()
    {
        // Initialize the player list from the game
        PlayersList = Players;
    }

    /// <summary>
    /// Retrieves an entity or player from a network entity address.
    /// </summary>
    public static object? FromNetworkEntity(string address)
    {
        var split = address.Split('-');
        if (split.Length != 2)
        {
            return null;
        }

        if (split[0] == "player")
        {
            var playerId = int.Parse(split[1]);
            return PlayersList.FirstOrDefault(p => p.Character.NetworkId == playerId);
        }
        else if (split[0] == "entity")
        {
            var entity = int.Parse(split[1]);
            return Entity.FromNetworkId(entity);
        }

        return null;
    }

    /// <summary>
    /// Retrieves the network ID of a given entity.
    /// </summary>
    public static int GetEntityAddress(Entity entity)
    {
        if (entity == null)
        {
            Debug.WriteLine("Entity is null");
            return -1;
        }

        return entity.NetworkId;
    }

    /// <summary>
    /// Converts a player into a network entity address.
    /// </summary>
    public static string ToNetworkEntity(Player player)
    {
        return $"player-{GetEntityAddress(player.Character)}";
    }

    /// <summary>
    /// Converts an entity into a network entity address.
    /// </summary>
    public static string ToNetworkEntity(Entity entity)
    {
        return $"entity-{GetEntityAddress(entity)}";
    }
}
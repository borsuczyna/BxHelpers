using System;
using System.Security.Cryptography;
using System.Text;

namespace BxHelpers;

/// <summary>
/// Utility class for generating and validating secure hashes.
/// </summary>
public static class BxHash
{
    private static readonly string SecretKey = BxDefines.HashSecretKey;

    /// <summary>
    /// Generates a unique hash using HMACSHA256.
    /// </summary>
    public static string Generate()
    {
        using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(SecretKey)))
        {
            var guid = Guid.NewGuid().ToString();
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(guid));
            return $"{guid}:{Convert.ToBase64String(hash)}";
        }
    }

    /// <summary>
    /// Validates a given hash by recomputing and comparing it.
    /// </summary>
    public static bool Validate(string hash)
    {
        var parts = hash.Split(':');
        if (parts.Length != 2) return false;

        string guid = parts[0];
        string receivedHash = parts[1];

        using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(SecretKey)))
        {
            var computedHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(guid)));
            return computedHash == receivedHash;
        }
    }
}
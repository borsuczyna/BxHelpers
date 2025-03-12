using System;
using System.Text;
using BxHelpers;

/// <summary>
/// Utility class for encoding and decoding strings securely.
/// </summary>
public static class BxEncoder
{
    private static readonly string SecretKey = BxDefines.EncodeHash;

    /// <summary>
    /// Simple XOR-based encoding (not cryptographically secure).
    /// </summary>
    public static string Encode(string input)
    {
        var key = Encoding.UTF8.GetBytes(SecretKey);
        var data = Encoding.UTF8.GetBytes(input);
        
        for (int i = 0; i < data.Length; i++)
        {
            data[i] ^= key[i % key.Length];
        }
        
        return Convert.ToBase64String(data);
    }

    /// <summary>
    /// Decodes a given encoded string by reversing the XOR operation.
    /// </summary>
    public static string Decode(string encoded)
    {
        var key = Encoding.UTF8.GetBytes(SecretKey);
        var data = Convert.FromBase64String(encoded);
        
        for (int i = 0; i < data.Length; i++)
        {
            data[i] ^= key[i % key.Length];
        }
        
        return Encoding.UTF8.GetString(data, 0, data.Length);
    }
}
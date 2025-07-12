using System.Security.Cryptography;

namespace Server.service.utils;

public static class RandomUtils
{
    //Generates a random string of characters from the given set
    public static string GenerateRandomString(int len, string chars)
    {
        char[] result = new char[len];
        using var rng = RandomNumberGenerator.Create();
        var buffer = new byte[len];

        rng.GetBytes(buffer);

        for (int i = 0; i < len; i++)
        {
            result[i] = chars[buffer[i] % chars.Length];
        }

        return new string(result);
    }
}
namespace Server.service;

using System.Security.Cryptography;

public static class PasswordHasher
{

    /// <summary>
    /// generates a hashed password that can only be decodeded using verifypassword,
    /// two passwords hashed by this will not be the same.
    /// </summary>
    /// <returns>Hashed Passoword</returns>
    public static string HashPassword(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(16);

        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
        byte[] hash = pbkdf2.GetBytes(32);

        byte[] hashBytes = new byte[48]; 
        Buffer.BlockCopy(salt, 0, hashBytes, 0, 16);
        Buffer.BlockCopy(hash, 0, hashBytes, 16, 32);

        return Convert.ToBase64String(hashBytes);
    }

    /// <summary>
    /// Not sure how this works but decrypts the password? wtf does Rfc2898DeriveBytes stand for.
    /// credits to microsoft for making this algorithim public
    /// </summary>
    /// <param name="password">password being checked</param>
    /// <param name="storedHash">hashed password from database</param>
    /// <returns>true if they are equal, false else</returns>
    public static bool VerifyPassword(string password, string storedHash)
    {
        byte[] hashBytes = Convert.FromBase64String(storedHash);

        byte[] salt = new byte[16];
        Buffer.BlockCopy(hashBytes, 0, salt, 0, 16);

        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
        byte[] hash = pbkdf2.GetBytes(32);

        for (int i = 0; i < 32; i++)
        {
            if (hashBytes[i + 16] != hash[i])
                return false;
        }

        return true;
    }
}
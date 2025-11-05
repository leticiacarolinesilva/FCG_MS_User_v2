using System.Security.Cryptography;

namespace FCG_MS_Users.Domain.Helper;

public static class PasswordHelper
{
    private const int SaltSizeBytes = 16;
    private const int KeySizeBytes = 32;
    private const int Iterations = 100_000;


    public static string HashPassword(string password)
    {
        if (password == null) throw new ArgumentNullException(nameof(password));

        using var rng = RandomNumberGenerator.Create();
        var salt = new byte[SaltSizeBytes];
        rng.GetBytes(salt);

        var key = Rfc2898DeriveKey(password, salt, Iterations, KeySizeBytes);

        var saltB64 = Convert.ToBase64String(salt);
        var keyB64 = Convert.ToBase64String(key);
        return $"{Iterations}.{saltB64}.{keyB64}";
    }

    public static bool VerifyPassword(string password, string storedHash)
    {
        if (string.IsNullOrWhiteSpace(storedHash) || password == null) return false;

        var parts = storedHash.Split('.');
        if (parts.Length != 3) return false;

        if (!int.TryParse(parts[0], out var iterations)) return false;
        var salt = Convert.FromBase64String(parts[1]);
        var expectedKey = Convert.FromBase64String(parts[2]);

        var actualKey = Rfc2898DeriveKey(password, salt, iterations, expectedKey.Length);

        return CryptographicOperations.FixedTimeEquals(actualKey, expectedKey);
    }

    private static byte[] Rfc2898DeriveKey(string password, byte[] salt, int iterations, int keySizeBytes)
    {
        using var deriveBytes = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
        return deriveBytes.GetBytes(keySizeBytes);
    }
}



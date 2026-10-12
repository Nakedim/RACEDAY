using BCrypt.Net;
using Org.BouncyCastle.Crypto.Generators;
using System.Security.Cryptography;
namespace RaceDay.Api.Services;


public class PswHasher
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 100000;
    private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA512;

         

  public static string HashPassword(string password)
    {
  
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithm);

        byte[] hash = pbkdf2.GetBytes(KeySize);
        return $"{Iterations}:{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
    }
    public static bool VerifyPassword(string password, string storedHashFormat)
    {

        var parts = storedHashFormat.Split(':');
        if (parts.Length != 3) return false;

        int iterations = int.Parse(parts[0]);
        byte[] salt = Convert.FromBase64String(parts[1]);
        byte[] storedHash = Convert.FromBase64String(parts[2]);

        // 2. Hash the incoming password using the exact same parameters
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithm);
        byte[] testHash = pbkdf2.GetBytes(KeySize);

        // 3. Protect against timing attacks using FixedTimeEquals
        return CryptographicOperations.FixedTimeEquals(storedHash, testHash);
    }

}
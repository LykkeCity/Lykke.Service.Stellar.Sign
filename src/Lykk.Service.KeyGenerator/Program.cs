using System;
using stellar_dotnet_sdk;

namespace Lykk.Service.Stellar.KeyGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var stellarPrivateKeyPair = KeyPair.Random();
            Console.WriteLine($"Seed(PrivateKey): {stellarPrivateKeyPair.SecretSeed}");
            Console.WriteLine($"Address(PublicAddress): {stellarPrivateKeyPair.Address}");
            Console.ReadLine();
        }
    }
}

using System;
using Chaos.NaCl;
using JetBrains.Annotations;
using Lykke.Service.Stellar.Sign.Core.Services;
using Lykke.Service.Stellar.Sign.Core.Encoding;
using Lykke.Service.Stellar.Sign.Core.Domain;
using stellar_dotnet_sdk;
using stellar_dotnet_sdk.xdr;
using FormatException = System.FormatException;
using Transaction = stellar_dotnet_sdk.Transaction;

namespace Lykke.Service.Stellar.Sign.Services
{
    public class StellarService : IStellarService
    {
        private readonly string _depositBaseAddress;

        [UsedImplicitly]
        public StellarService(string network,
                              string depositBaseAddress)
        {
            if (network != "Test SDF Network ; September 2015")
                Network.UsePublicNetwork();
            else
                Network.UseTestNetwork();

            _depositBaseAddress = depositBaseAddress;
        }

        public string GetDepositBaseAddress()
        {
            return _depositBaseAddress;
        }

        public Core.Domain.Stellar.KeyPair GenerateKeyPair()
        {
            var keyPair = KeyPair.Random();
            return new Core.Domain.Stellar.KeyPair
            {
                Seed = keyPair.SecretSeed,
                Address = keyPair.Address
            };
        }

        public string GenerateRandomMemoText()
        {
            var guid = Guid.NewGuid().ToByteArray();
            var memoText = Base3264Encoding.ToZBase32(guid);
            return memoText;
        }

        public string SignTransaction(string[] seeds, string xdrBase64)
        {
            byte[] xdr;
            try
            {
                xdr = Convert.FromBase64String(xdrBase64);
            }
            catch (FormatException ex)
            {
                throw new ArgumentException("Invalid base64 encoded transaction XDR", nameof(xdrBase64), ex);
            }

            var inputStream = new XdrDataInputStream(xdr);
            var xdrTransaction = stellar_dotnet_sdk.xdr.TransactionEnvelope.Decode(inputStream);
            var txV1 = xdrTransaction;
            var tx = TransactionBuilder.FromEnvelopeXdr(txV1);
            var txHash = GetTransactionHash(tx);

            var seed = seeds[0];
            if (Constants.NoPrivateKey.Equals(seed, StringComparison.Ordinal))
            {
            }
            else
            {
                try
                {
                    var signer = KeyPair.FromSecretSeed(seed);
                    tx.Sign(signer);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("Invalid base64 encoded transaction XDR", nameof(xdrBase64), ex);
                }
                
            }

            return tx.ToEnvelopeXdrBase64(TransactionBase.TransactionXdrVersion.V1);
        }

        private static byte[] GetTransactionHash(TransactionBase tx)
        {
            return tx.Hash();
        }
    }
}

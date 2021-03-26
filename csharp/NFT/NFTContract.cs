using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System;
using System.ComponentModel;
using System.Numerics;
using Neo;

namespace NFT
{
    [DisplayName("NFT Contract")]
    [ManifestExtra("Author", "NEO")]
    [ManifestExtra("Email", "developer@neo.org")]
    [ManifestExtra("Description", "This is a Neo3 NFT Contract")]
    partial class NFTContract : SmartContract
    {
        [DisplayName("MintToken")]
        public static event Action<UInt160, ByteString, byte[]> MintTokenNotify;

        [DisplayName("BurnToken")]
        public static event Action<UInt160, ByteString, BigInteger> BurnTokenNotify;

        [DisplayName("Transfer")]
        public static event Action<UInt160, UInt160, BigInteger, ByteString> TransferNotify;

        private static readonly UInt160 superAdmin = "NMA2FKN8up2cEwaJgtmAiDrZWB69ApnDfp".ToScriptHash();

        private static StorageContext Context => Storage.CurrentContext;

        private const byte Prefix_TotalSupply = 10;
        private const byte Prefix_TokenOwner = 11;
        private const byte Prefix_TokenBalance = 12;
        private const byte Prefix_Properties = 13;
        private const byte Prefix_TokensOf = 14;

        private const int TOKEN_DECIMALS = 8;
        private const int FACTOR = 100_000_000;


        public static string Symbol()
        {
            return "NFT";
        }

        public static string[] SupportedStandards()
        {
            return new string[] { "NEP-10", "NEP-11" };
        }

        private static byte[] CreateStorageKey(byte prefix, ByteString key)
        {
            return CreateStorageKey(prefix, (byte[])key);
        }

        private static byte[] CreateStorageKey(byte prefix, byte[] key)
        {
            return prefix.ToByteArray().Concat(key);
        }

        public static BigInteger TotalSupply()
        {
            return ToBigInteger(StorageGet(Prefix_TotalSupply.ToByteArray()));
        }

        private static void SetTotalSupply(BigInteger total)
        {
            StoragePut(Prefix_TotalSupply.ToByteArray(), total);
        }

        public static int Decimals()
        {
            return TOKEN_DECIMALS;
        }

        public static Iterator<byte[]> OwnerOf(ByteString tokenid)
        {
            return (Iterator<byte[]>)Storage.Find(Context, CreateStorageKey(Prefix_TokenOwner, tokenid));
        }

        public static Iterator<byte[]> TokensOf(UInt160 owner)
        {
            if (!owner.IsValid) throw new FormatException("The parameter 'owner' should be 20-byte address.");
            return (Iterator<byte[]>)Storage.Find(Context, CreateStorageKey(Prefix_TokensOf, owner));
        }

        public static string Properties(ByteString tokenid)
        {
            return StorageGet(CreateStorageKey(Prefix_Properties, tokenid));
        }

        public static bool Mint(ByteString tokenId, UInt160 owner, byte[] properties)
        {
            if (!Runtime.CheckWitness(superAdmin)) return false;

            if (!owner.IsValid) throw new FormatException("The parameter 'owner' should be 20-byte address.");
            if (properties.Length > 2048) throw new FormatException("The length of 'properties' should be less than 2048.");

            StorageMap tokenOwnerMap = Storage.CurrentContext.CreateMap(CreateStorageKey(Prefix_TokenOwner, tokenId));
            if (tokenOwnerMap.Get(owner) != null) return false;

            StorageMap tokenOfMap = Storage.CurrentContext.CreateMap(CreateStorageKey(Prefix_TokensOf, owner));
            StoragePut(CreateStorageKey(Prefix_Properties, tokenId), properties);
            tokenOwnerMap.Put(owner, owner);
            tokenOfMap.Put(tokenId, tokenId);

            var totalSupply = TotalSupply();
            SetTotalSupply(totalSupply + FACTOR);


            StorageMap tokenBalanceMap = Storage.CurrentContext.CreateMap(CreateStorageKey(Prefix_TokenBalance, owner));
            tokenBalanceMap.Put(tokenId, FACTOR);

            // Notify
            MintTokenNotify(owner, tokenId, properties);
            return true;
        }

        public static bool Burn(ByteString tokenId, UInt160 owner, BigInteger amount)
        {
            if (!owner.IsValid) throw new FormatException("The parameter 'owner' should be 20-byte address.");

            if (amount < 0 || amount > FACTOR) throw new FormatException("The parameters 'amount' is out of range.");
            if (amount == 0) return true;
            if (!Runtime.CheckWitness(owner)) return false;

            var tokenBalanceMap = Storage.CurrentContext.CreateMap(CreateStorageKey(Prefix_TokenBalance, owner));
            var balanceValue = tokenBalanceMap.Get(tokenId);
            var balance = balanceValue is null ? 0 : (BigInteger)balanceValue;
            if (balance < amount) return false;

            var totalSupply = TotalSupply();
            balance -= amount;
            totalSupply -= amount;
            tokenBalanceMap.Put(tokenId, balance);
            SetTotalSupply(totalSupply);

            // Notify
            BurnTokenNotify(owner, tokenId, amount);
            return true;
        }

        public static BigInteger BalanceOf(UInt160 owner, ByteString tokenid)
        {
            if (!owner.IsValid) throw new FormatException("The parameter 'owner' should be 20-byte address.");

            if (tokenid is null)
            {
                var iterator = Storage.Find(Context, CreateStorageKey(Prefix_TokenBalance, owner));
                BigInteger result = 0;
                while (iterator.Next())
                    result += (BigInteger)iterator.Value;
                return result;
            }

            var value = Storage.CurrentContext.CreateMap(CreateStorageKey(Prefix_TokenBalance, owner)).Get((ByteString)tokenid);
            return value is null ? 0 : (BigInteger)value;
        }

        public static bool Transfer(UInt160 from, UInt160 to, BigInteger amount, ByteString tokenId)
        {
            if (!from.IsValid) throw new FormatException("The parameter 'from' should be 20-byte address.");
            if (!to.IsValid) throw new FormatException("The parameter 'to' should be 20-byte address.");

            if (amount < 0 || amount > FACTOR) throw new FormatException("The parameters 'amount' is out of range.");
            if (!Runtime.CheckWitness(from)) return false;

            if (from.Equals(to))
            {
                TransferNotify(from, to, amount, tokenId);
                return true;
            }

            StorageMap fromTokenBalanceMap = Storage.CurrentContext.CreateMap(CreateStorageKey(Prefix_TokenBalance, from));
            StorageMap toTokenBalanceMap = Storage.CurrentContext.CreateMap(CreateStorageKey(Prefix_TokenBalance, to));
            StorageMap tokenOwnerMap = Storage.CurrentContext.CreateMap(CreateStorageKey(Prefix_TokenOwner, tokenId));
            StorageMap fromTokensOfMap = Storage.CurrentContext.CreateMap(CreateStorageKey(Prefix_TokensOf, from));
            StorageMap toTokensOfMap = Storage.CurrentContext.CreateMap(CreateStorageKey(Prefix_TokensOf, to));

            var fromTokenBalance = ToBigInteger(fromTokenBalanceMap.Get(tokenId));
            if (fromTokenBalance == 0 || fromTokenBalance < amount) return false;
            var fromNewBalance = fromTokenBalance - amount;
            if (fromNewBalance == 0)
            {
                tokenOwnerMap.Delete(from);
                fromTokensOfMap.Delete(tokenId);
            }
            fromTokenBalanceMap.Put(tokenId, fromNewBalance);

            var toTokenBalance = ToBigInteger(toTokenBalanceMap.Get(tokenId));
            if (toTokenBalance == 0 && amount > 0)
            {
                tokenOwnerMap.Put(to, to);
                toTokenBalanceMap.Put(tokenId, amount);
                toTokensOfMap.Put(tokenId, tokenId);
            }
            else
            {
                toTokenBalanceMap.Put(tokenId, toTokenBalance + amount);
            }

            // Notify
            TransferNotify(from, to, amount, tokenId);
            return true;
        }

        public static bool Migrate(ByteString script, string manifest)
        {
            if (!Runtime.CheckWitness(superAdmin))
            {
                return false;
            }
            if (script.Length == 0 || manifest.Length == 0)
            {
                return false;
            }
            ContractManagement.Update(script, manifest);
            return true;
        }

        public static bool Destroy()
        {
            if (!Runtime.CheckWitness(superAdmin))
            {
                return false;
            }

            ContractManagement.Destroy();
            return true;
        }
    }
}

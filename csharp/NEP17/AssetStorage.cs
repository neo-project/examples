using Neo.SmartContract.Framework.Services;
using System.Numerics;

namespace Neo.SmartContract.Examples
{
    public static class AssetStorage
    {
        public static readonly string mapName = "asset";

        public static readonly StorageMap assetMap = new StorageMap(Storage.CurrentContext, mapName);

        public static void Increase(UInt160 key, BigInteger value) => Put(key, Get(key) + value);

        public static void Enable() => assetMap.Put("enable", 1);

        public static void Disable() => assetMap.Put("enable", 0);

        public static void Reduce(UInt160 key, BigInteger value)
        {
            var oldValue = Get(key);
            if (oldValue == value)
                Remove(key);
            else
                Put(key, oldValue - value);
        }

        public static void Put(UInt160 key, BigInteger value) => assetMap.Put(key, value);

        public static BigInteger Get(UInt160 key)
        {
            var value = assetMap.Get(key);
            return value is null ? 0 : (BigInteger)value;
        }

        public static bool GetPaymentStatus() => ((BigInteger)assetMap.Get("enable")).Equals(1);

        public static void Remove(UInt160 key) => assetMap.Delete(key);
    }
}

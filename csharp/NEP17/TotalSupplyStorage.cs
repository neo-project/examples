using Neo.SmartContract.Framework.Services;
using System.Numerics;

namespace Neo.SmartContract.Examples
{
    public static class TotalSupplyStorage
    {
        public static readonly string mapName = "contract";

        public static readonly string key = "totalSupply";

        public static readonly StorageMap contractMap = new StorageMap(Storage.CurrentContext, mapName);

        public static void Increase(BigInteger value) => Put(Get() + value);

        public static void Reduce(BigInteger value) => Put(Get() - value);

        public static void Put(BigInteger value) => contractMap.Put(key, value);

        public static BigInteger Get()
        {
            var value = contractMap.Get(key);
            return value is null ? 0 : (BigInteger)value;
        }
    }
}

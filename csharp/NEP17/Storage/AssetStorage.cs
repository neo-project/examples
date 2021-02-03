using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System.Numerics;

namespace Template.NEP17.CSharp
{
    public static class AssetStorage
    {
        public static readonly string mapName = "asset";

        public static void Increase(UInt160 key, BigInteger value) => Put(key, Get(key) + value);

        public static void Enable() => Storage.CurrentContext.CreateMap(mapName).Put("enable", 1);

        public static void Disable() => Storage.CurrentContext.CreateMap(mapName).Put("enable", 0);

        public static void Reduce(UInt160 key, BigInteger value)
        {
            var oldValue = Get(key);
            if (oldValue == value)
                Remove(key);
            else
                Put(key, oldValue - value);
        }

        public static void Put(UInt160 key, BigInteger value) => Storage.CurrentContext.CreateMap(mapName).Put(key, value);

        public static BigInteger Get(UInt160 key) => (BigInteger)Storage.CurrentContext.CreateMap(mapName).Get(key);

        public static bool GetPaymentStatus()
        {
            return Storage.CurrentContext.CreateMap(mapName).Get("enable").Equals(1);
        }

        public static void Remove(UInt160 key) => Storage.CurrentContext.CreateMap(mapName).Delete(key);
    }
}

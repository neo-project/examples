using Neo.SmartContract.Framework;
using System.Numerics;

namespace Neo.SmartContract.Examples
{
    public static class AssetStorage
    {
        public static readonly Map<string, BigInteger> asset = new();

        public static void Increase(UInt160 key, BigInteger value) => Put(key, Get(key) + value);

        public static void Enable() => asset["enable"] = 1;

        public static void Disable() => asset["enable"] = 0;

        public static void Reduce(UInt160 key, BigInteger value)
        {
            var oldValue = Get(key);
            if (oldValue == value)
                Remove(key);
            else
                Put(key, oldValue - value);
        }

        public static void Put(UInt160 key, BigInteger value) => asset[key] = value;

        public static BigInteger Get(UInt160 key) => asset[key];

        public static bool GetPaymentStatus() => asset["enable"].Equals(1);

        public static void Remove(UInt160 key) => asset.Remove(key);
    }
}

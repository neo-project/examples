using Neo.SmartContract.Framework;
using System.Numerics;

namespace Neo.SmartContract.Examples
{
    public static class TotalSupplyStorage
    {
        public static readonly Map<string, BigInteger> contract = new();

        public static readonly string key = "totalSupply";

        public static void Increase(BigInteger value) => Put(Get() + value);

        public static void Reduce(BigInteger value) => Put(Get() - value);

        public static void Put(BigInteger value) => contract[key] = value;

        public static BigInteger Get() => contract[key];
    }
}

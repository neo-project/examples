using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System;
using System.Numerics;

namespace EventExample
{
    [ManifestExtra("Author", "Neo")]
    [ManifestExtra("Email", "dev@neo.org")]
    [ManifestExtra("Description", "This is a contract example")]
    public class Contract1 : SmartContract
    {
        public static event Action<string, BigInteger> Event1;

        public static event Action<byte[], string> Event2;

        public static void Run()
        {
            Event1("hello", 123);
            Event2(new byte[] { 0x01 }, "hello");
        }
    }
}

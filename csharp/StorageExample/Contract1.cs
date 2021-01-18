using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System;
using System.Numerics;

namespace StorageExample
{
    [ManifestExtra("Author", "Neo")]
    [ManifestExtra("Email", "dev@neo.org")]
    [ManifestExtra("Description", "This is a contract example")]
    public class Contract1 : SmartContract
    {
        //TODO: Replace it with your own address.
        internal static readonly UInt160 InitialOwnerScriptHash = "NiNmXL8FjEUEs1nfX9uHFBNaenxDHJtmuB".ToScriptHash();

        private static bool IsOwner() => Runtime.CheckWitness(OwnerStorage.Get());

        public static bool Verify() => IsOwner();

        public static void Increase() => AssetStorage.Increase(OwnerStorage.Get(), 100);

        public static void Reduce() => AssetStorage.Reduce(OwnerStorage.Get(), 50);


        public static void GetBalance() => AssetStorage.Get(OwnerStorage.Get());

        public static void Blacklist(UInt160 account) => BlacklistStorage.Add(account, Blockchain.GetHeight());

        public static bool IsBlacklisted(UInt160 account) => BlacklistStorage.Exist(account);
    }
}

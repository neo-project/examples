using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.Numerics;

namespace Neo.SmartContract.Examples
{
    [ManifestExtra("Author", "Neo")]
    [ManifestExtra("Email", "dev@neo.org")]
    [ManifestExtra("Description", "This is a NEP17 example")]
    [SupportedStandards("NEP-17")]
    [ContractPermission("*", "onNEP17Payment")]
    public partial class NEP17Demo : Nep17Token
    {
        [InitialValue("NhGobEnuWX5rVdpnuZZAZExPoRs5J6D2Sb", ContractParameterType.Hash160)]
        private static readonly UInt160 owner = default;
        // Prefix_TotalSupply = 0x00; Prefix_Balance = 0x01;
        private const byte Prefix_Contract = 0x02;
        public static readonly StorageMap ContractMap = new StorageMap(Storage.CurrentContext, Prefix_Contract);
        private static readonly byte[] ownerKey = "owner".ToByteArray();
        private static bool IsOwner() => Runtime.CheckWitness(GetOwner());
        public override byte Decimals() => Factor();
        public override string Symbol() => "NEP17";

        public static byte Factor() => 8;

        public static void _deploy(object data, bool update)
        {
            if (update) return;
            ContractMap.Put(ownerKey, owner);
            Nep17Token.Mint(owner, 100000000 * BigInteger.Pow(10, Factor()));
        }

        public static UInt160 GetOwner()
        {
            return (UInt160)ContractMap.Get(ownerKey);
        }

        public static new void Mint(UInt160 account, BigInteger amount)
        {
            if (!IsOwner()) throw new InvalidOperationException("No Authorization!");
            Nep17Token.Mint(account, amount);
        }

        public static new void Burn(UInt160 account, BigInteger amount)
        {
            if (!IsOwner()) throw new InvalidOperationException("No Authorization!");
            Nep17Token.Burn(account, amount);
        }

        public static bool Update(ByteString nefFile, string manifest)
        {
            if (!IsOwner()) throw new InvalidOperationException("No Authorization!");
            ContractManagement.Update(nefFile, manifest, null);
            return true;
        }

        public static bool Destroy()
        {
            if (!IsOwner()) throw new InvalidOperationException("No Authorization!");
            ContractManagement.Destroy();
            return true;
        }
    }
}

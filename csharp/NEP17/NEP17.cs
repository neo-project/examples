using Neo.SmartContract.Framework;
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
        private static readonly byte[] ownerKey = "owner".ToByteArray();
        private static bool IsOwner() => Runtime.CheckWitness(GetOwner());
        public override byte Decimals() => 8;
        public override string Symbol() => "NEP17";

        public static void _deploy(object data, bool update)
        {
            if (update) return;
            Storage.Put(Storage.CurrentContext, ownerKey, owner);
        }

        public static UInt160 GetOwner()
        {
            return (UInt160)Storage.Get(Storage.CurrentContext, ownerKey);
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

        public static bool Update(ByteString nefFile, string manifest, object data = null)
        {
            if (!IsOwner()) throw new InvalidOperationException("No Authorization!");
            ContractManagement.Update(nefFile, manifest, data);
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

using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System;

namespace Template.NEP17.CSharp
{
    public partial class NEP17 : SmartContract
    {
        public static void _deploy(bool update)
        {
            if (update) return;
            if (TotalSupplyStorage.Get() > 0) throw new Exception("Contract has been deployed.");

            TotalSupplyStorage.Increase(InitialSupply);
            AssetStorage.Increase(Owner, InitialSupply);

            OnTransfer(null, Owner, InitialSupply);
        }

        public static void Update(byte[] nefFile, string manifest)
        {
            if (!IsOwner()) throw new Exception("No authorization.");
            ContractManagement.Update(nefFile, manifest);
        }

        public static void Destroy()
        {
            if (!IsOwner()) throw new Exception("No authorization.");
            ContractManagement.Destroy();
        }

        public static void EnablePayment()
        {
            if (!IsOwner()) throw new Exception("No authorization.");
            AssetStorage.Enable();
        }

        public static void DisablePayment()
        {
            if (!IsOwner()) throw new Exception("No authorization.");
            AssetStorage.Disable();
        }

        private static bool IsOwner() => Runtime.CheckWitness(Owner);
    }
}

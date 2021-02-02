using Neo;
using Neo.SmartContract;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using Neo.SmartContract.Framework.Services.System;
using System;
using System.Numerics;

namespace Template.NEP17.CSharp
{
    public partial class NEP17 : SmartContract
    {
        public static BigInteger TotalSupply() => TotalSupplyStorage.Get();

        public static BigInteger BalanceOf(UInt160 account)
        {
            if (!ValidateAddress(account)) throw new Exception("The parameters account SHOULD be a 20-byte non-zero address.");
            return AssetStorage.Get(account);
        }

        public static bool Transfer(UInt160 from, UInt160 to, BigInteger amount, object data)
        {
            if (!ValidateAddress(from) || !ValidateAddress(to)) throw new Exception("The parameters from and to SHOULD be 20-byte non-zero addresses.");
            if (amount <= 0) throw new Exception("The parameter amount MUST be greater than 0.");
            if (!Runtime.CheckWitness(from) && !from.Equals(ExecutionEngine.CallingScriptHash)) throw new Exception("No authorization.");
            if (AssetStorage.Get(from) < amount) throw new Exception("Insufficient balance.");
            if (from == to) return true;

            AssetStorage.Reduce(from, amount);
            AssetStorage.Increase(to, amount);

            OnTransfer(from, to, amount);

            // Validate payable
            if (IsDeployed(to)) Contract.Call(to, "onNEP17Payment", CallFlags.ReadOnly, new object[] { from, amount, data });
            return true;
        }
    }
}

using Neo.SmartContract.Framework.Services.Neo;
using Neo.SmartContract.Framework.Services.System;
using System;
using System.Numerics;

namespace Neo.SmartContract.Examples
{
    partial class NEP17Demo
    {
        public static BigInteger TotalSupply() => TotalSupplyStorage.Get();

        public static BigInteger BalanceOf(UInt160 account) => AssetStorage.Get(account);

        public static bool Transfer(UInt160 from, UInt160 to, BigInteger amount, object data)
        {
            if (amount <= 0) throw new Exception("The parameter amount MUST be greater than 0.");
            if (!Runtime.CheckWitness(from) && !from.Equals(ExecutionEngine.CallingScriptHash)) throw new Exception("No authorization.");
            if (AssetStorage.Get(from) < amount) throw new Exception("Insufficient balance.");
            if (from == to) return true;

            AssetStorage.Reduce(from, amount);
            AssetStorage.Increase(to, amount);

            OnTransfer(from, to, amount);

            // Validate payable
            if (ContractManagement.GetContract(to) != null)
                Contract.Call(to, "onNEP17Payment", CallFlags.All, new object[] { from, amount, data });
            return true;
        }
    }
}

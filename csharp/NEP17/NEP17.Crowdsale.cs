using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.Numerics;

namespace Neo.SmartContract.Examples
{
    partial class NEP17Demo
    {
        public static void OnNEP17Payment(UInt160 from, BigInteger amount, object data)
        {
            if (IsPayable())
            {
                if (Runtime.CallingScriptHash == NEO.Hash)
                {
                    Mint(amount * TokensPerNEO);
                }
                else if (Runtime.CallingScriptHash == GAS.Hash)
                {
                    if (from != null) Mint(amount * TokensPerGAS);
                }
                else
                {
                    throw new Exception("Wrong calling script hash");
                }
            }
            else
            {
                throw new Exception("Payment is disable on this contract!");
            }
        }

        private static void Mint(BigInteger amount)
        {
            var totalSupply = TotalSupplyStorage.Get();

            var avaliable_supply = MaxSupply - totalSupply;

            if (amount <= 0) throw new Exception("Amount must be greater than zero.");
            if (amount > avaliable_supply) throw new Exception("Insufficient supply for mint tokens.");

            Transaction tx = (Transaction)Runtime.ScriptContainer;
            AssetStorage.Increase(tx.Sender, amount);
            TotalSupplyStorage.Increase(amount);

            OnTransfer(null, tx.Sender, amount);
        }
    }
}

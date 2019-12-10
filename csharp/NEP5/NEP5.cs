using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using Neo.SmartContract.Framework.Services.System;
using System;
using System.ComponentModel;
using System.Numerics;

namespace NEP5
{
    public class NEP5 : SmartContract
    {
        [DisplayName("Transfer")]
        public static event Action<byte[], byte[], BigInteger> Transferred;

        private static readonly byte[] Owner = "Ae2d6qj91YL3LVUMkza7WQsaTYjzjHm4z1".ToScriptHash(); //Owner Address
        private static readonly byte[] callingscript = ExecutionEngine.CallingScriptHash;

        public static BigInteger BalanceOf(byte[] account)
        {
            if (account.Length != 20)
                throw new InvalidOperationException("The parameter account SHOULD be 20-byte addresses.");
            return Storage.Get(account)?.AsBigInteger() ?? 0;
        }

        public static byte Decimals() => 8;

        public static string Name() => "MyToken"; //name of the token

        public static string Symbol() => "MYT"; //symbol of the token

        public static string[] SupportedStandards() => new string[] { "NEP-5", "NEP-10" };

        public static BigInteger TotalSupply()
        {
            return Storage.Get("totalSupply")?.AsBigInteger() ?? 0;
        }

        public static bool Transfer(byte[] from, byte[] to, BigInteger amount)
        {
            if (Runtime.Trigger != TriggerType.Application) throw new InvalidOperationException();

            //Check parameters
            if (from.Length != 20 || to.Length != 20)
                throw new InvalidOperationException("The parameters from and to SHOULD be 20-byte addresses.");
            if (amount < 0)
                throw new InvalidOperationException("The parameter amount MUST be greater than 0.");
            if (Blockchain.GetContract(to)?.IsPayable == false)
                return false;
            if (from != callingscript && !Runtime.CheckWitness(from))
                return false;

            StorageContext context = Storage.CurrentContext;
            var fromAmount = Storage.Get(context, from)?.AsBigInteger() ?? 0;
            if (fromAmount < amount) return false;

            if (from != to && amount > 0)
            {
                //Reduce payer balances
                if (fromAmount == amount)
                    Storage.Delete(context, from);
                else
                    Storage.Put(context, from, fromAmount - amount);

                //Increase the payee balance
                var toAmount = Storage.Get(context, to)?.AsBigInteger() ?? 0;
                Storage.Put(context, to, toAmount + amount);
            }

            Transferred(from, to, amount);
            return true;
        }

        public static bool Verify()
        {
            return Runtime.CheckWitness(Owner);
        }
    }
}

using AntShares.SmartContract.Framework;
using AntShares.SmartContract.Framework.Services.AntShares;
using AntShares.SmartContract.Framework.Services.System;
using System.Numerics;

namespace AntShares.SmartContract
{
    public class ContractAsset : FunctionCode
    {
        public static object Main(string operation, params object[] args)
        {
            switch (operation)
            {
                case "create":
                    return Create((string)args[0], (BigInteger)args[1], (byte[])args[2]);
                case "transfer":
                    return Transfer((byte[])args[0], (byte[])args[1], (BigInteger)args[2]);
                default:
                    return false;
            }
        }

        public static byte[] Create(string name, BigInteger amount, byte[] owner)
        {
            Contract contract = Blockchain.GetContract(ExecutionEngine.ExecutingScriptHash);
            byte[] script = contract.Script.Concat(name.AsByteArray());
            contract = Contract.Create(script, new byte[] { 5, 16 }, 5, true, name, "1.0.0-preview1", owner.AsString(), "", name);
            Storage.Put(contract.StorageContext, owner, amount.ToByteArray());
            return Hash160(contract.Script);
        }

        public static bool Transfer(byte[] from, byte[] to, BigInteger value)
        {
            if (value <= 0) return false;
            if (from.Length != 33 || to.Length != 33) return false;
            if (!Runtime.CheckWitness(from)) return false;
            byte[] data = Storage.Get(Storage.CurrentContext, from);
            if (data == null) return false;
            BigInteger available = new BigInteger(data);
            if (available < value) return false;
            available -= value;
            Storage.Put(Storage.CurrentContext, from, available.ToByteArray());
            data = Storage.Get(Storage.CurrentContext, to);
            if (data == null)
                available = 0;
            else
                available = new BigInteger(data);
            available += value;
            Storage.Put(Storage.CurrentContext, to, available.ToByteArray());
            return true;
        }
    }
}

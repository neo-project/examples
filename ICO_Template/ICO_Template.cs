using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using Neo.SmartContract.Framework.Services.System;
using System;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract
{
    public class ICO_Template : Framework.SmartContract
    {
        //Token Settings
        public static string Name() => "name of the token";
        public static string Symbol() => "SymbolOfTheToken";
        //public key or script hash
        public static readonly byte[] Owner = { 47, 60, 170, 33, 216, 40, 148, 2, 242, 150, 9, 84, 154, 50, 237, 160, 97, 90, 55, 183 };
        public static byte Decimals() => 8;
        private const ulong factor = 100000000; //decided by Decimals()

        //ICO Settings
        private static readonly byte[] neo_asset_id = { 155, 124, 255, 218, 166, 116, 190, 174, 15, 147, 14, 190, 96, 133, 175, 144, 147, 229, 254, 86, 179, 74, 92, 34, 12, 205, 207, 110, 252, 51, 111, 197 };
        private const ulong total_amount = 100000000 * factor; // total token amount
        private const ulong pre_ico_cap = 30000000 * factor; // pre ico token amount
        private const ulong basic_rate = 1000 * factor;
        private const int ico_start_time = 1507824000;
        private const int ico_end_time = 1507910400;

        [DisplayName("transfer")]
        public static event Action<byte[], byte[], BigInteger> Transferred;

        [DisplayName("refund")]
        public static event Action<byte[], BigInteger> Refund;

        public static Object Main(string operation, params object[] args)
        {
            if (Runtime.Trigger == TriggerType.Verification)
            {
                if (Owner.Length == 20)
                {
                    return Runtime.CheckWitness(Owner);
                }
                else if (Owner.Length == 33)
                {
                    byte[] signature = operation.AsByteArray();
                    return VerifySignature(signature, Owner);
                }
            }
            else if (Runtime.Trigger == TriggerType.Application)
            {
                if (operation == "deploy") return Deploy();
                if (operation == "mintTokens") return MintTokens();
                if (operation == "totalSupply") return TotalSupply();
                if (operation == "name") return Name();
                if (operation == "symbol") return Symbol();
                if (operation == "transfer")
                {
                    if (args.Length != 3) return false;
                    byte[] from = (byte[])args[0];
                    byte[] to = (byte[])args[1];
                    BigInteger value = (BigInteger)args[2];
                    return Transfer(from, to, value);
                }
                if (operation == "balanceOf")
                {
                    if (args.Length != 1) return 0;
                    byte[] account = (byte[])args[0];
                    return BalanceOf(account);
                }
                if (operation == "decimals") return Decimals();
            }
            //convert transaction to output struct
            //you can choice refund or not refund
            Output output = InvestOuptut();
            if(output.Value > 0 && AssetCheck())
            {
                Refund(output.Sender, output.Value);
            }
            return false;
        }

        // initialization parameters, only once
        // 初始化参数
        public static bool Deploy()
        {
            byte[] total_supply = Storage.Get(Storage.CurrentContext, "totalSupply");
            if (total_supply.Length != 0) return false;
            Storage.Put(Storage.CurrentContext, Owner, pre_ico_cap);
            Storage.Put(Storage.CurrentContext, "totalSupply", pre_ico_cap);
            Transferred(null, Owner, pre_ico_cap);
            return true;
        }

        // The function MintTokens is only usable by the chosen wallet
        // contract to mint a number of tokens proportional to the
        // amount of neo sent to the wallet contract. The function
        // can only be called during the tokenswap period
        // 将众筹的neo转化为等价的ico代币
        public static bool MintTokens()
        {
            // if you contribute asset is not neo
            if(!AssetCheck())
            {
                return false;
            }
            // convert transaction to output struct
            Output output = InvestOuptut();
            // the current exchange rate between ico tokens and neo during the token swap period
            // 获取众筹期间ico token和neo间的转化率
            ulong swap_rate = CurrentSwapRate();
            // crowdfunding failure
            // 众筹失败
            if (swap_rate == 0)
            {
                Refund(output.Sender, output.Value);
                return false;
            }
            // you can obtain token amount
            ulong token = CurrentObtainToken(output.Sender, output.Value, swap_rate);
            if(token == 0)
            {
                return false;
            }
            // crowdfunding success
            // 众筹成功
            BigInteger balance = Storage.Get(Storage.CurrentContext, output.Sender).AsBigInteger();
            Storage.Put(Storage.CurrentContext, output.Sender, token + balance);
            BigInteger totalSupply = Storage.Get(Storage.CurrentContext, "totalSupply").AsBigInteger();
            Storage.Put(Storage.CurrentContext, "totalSupply", token + totalSupply);
            return true;
        }

        // get the total token supply
        // 获取已发行token总量
        public static BigInteger TotalSupply()
        {
            return Storage.Get(Storage.CurrentContext, "totalSupply").AsBigInteger();
        }

        // function that is always called when someone wants to transfer tokens.
        // 流转token调用
        public static bool Transfer(byte[] from, byte[] to, BigInteger value)
        {
            if (value <= 0) return false;
            if (!Runtime.CheckWitness(from)) return false;
            BigInteger from_value = Storage.Get(Storage.CurrentContext, from).AsBigInteger();
            if (from_value < value) return false;
            if (from_value == value)
                Storage.Delete(Storage.CurrentContext, from);
            else
                Storage.Put(Storage.CurrentContext, from, from_value - value);
            BigInteger to_value = Storage.Get(Storage.CurrentContext, to).AsBigInteger();
            Storage.Put(Storage.CurrentContext, to, to_value + value);
            Transferred(from, to, value);
            return true;
        }

        // get the account balance of another account with address
        // 根据地址获取token的余额
        public static BigInteger BalanceOf(byte[] address)
        {
            return Storage.Get(Storage.CurrentContext, address).AsBigInteger();
        }

        // The function CurrentSwapRate() returns the current exchange rate
        // between ico tokens and neo during the token swap period
        private static ulong CurrentSwapRate()
        {
            const int ico_duration = ico_end_time - ico_start_time;
            uint now = Blockchain.GetHeader(Blockchain.GetHeight()).Timestamp + 15*60;
            int time = (int)now - ico_start_time;
            if (time < 0)
            {
                return 0;
            }
            else if (time < 86400)
            {
                return basic_rate * 130 / 100;
            }
            else if (time < 259200)
            {
                return basic_rate * 120 / 100;
            }
            else if (time < 604800)
            {
                return basic_rate * 110 / 100;
            }
            else if (time < ico_duration)
            {
                return basic_rate;
            }
            else
            {
                return 0;
            }
        }

        //whether over invest capacity, you can obtain the token amount
        private static ulong CurrentObtainToken(byte[] sender, ulong value, ulong swap_rate)
        {
            ulong token = value / 100000000 * swap_rate;
            BigInteger total_supply = Storage.Get(Storage.CurrentContext, "totalSupply").AsBigInteger();
            BigInteger balance_token = total_amount - total_supply;
            if (balance_token <= 0)
            {
                Refund(sender, value);
                return 0;
            }
            else if (balance_token < token)
            {
                Refund(sender, (token - balance_token) / swap_rate * 100000000);
                token = (ulong) balance_token;
            }
            return token;
        }

        // asset check 
        private static bool AssetCheck()
        {
            Transaction tx = (Transaction)ExecutionEngine.ScriptContainer;
            TransactionOutput reference = tx.GetReferences()[0];
            // check whether asset is neo
            // 检查资产是否为neo
            // you can choice refund or not refund
            if (reference.AssetId != neo_asset_id) return false;
            return true;
        }

        private static Output InvestOuptut()
        {
            Transaction tx = (Transaction)ExecutionEngine.ScriptContainer;
            TransactionOutput reference = tx.GetReferences()[0];
            byte[] sender = reference.ScriptHash;
            TransactionOutput[] outputs = tx.GetOutputs();
            byte[] receiver = ExecutionEngine.ExecutingScriptHash;
            ulong value = 0;
            // get the total amount of Neo
            // 获取转入智能合约地址的Neo总量
            foreach (TransactionOutput output in outputs)
            {
                if (output.ScriptHash == receiver)
                {
                    value += (ulong)output.Value;
                }
            }
            return new Output { Sender = sender, Receiver = receiver, Value = value };
        }
    }
}

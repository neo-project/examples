using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;

namespace Neo.SmartContract
{
    public class Lock : FunctionCode
    {
        public static bool Main(uint timestamp, byte[] pubkey, byte[] signature)
        {
            Header header = Blockchain.GetHeader(Blockchain.GetHeight());
            if (timestamp > header.Timestamp) return false;
            return VerifySignature(pubkey, signature);
        }
    }
}

using Neo.SmartContract.Framework.Services.Neo;
using Neo.SmartContract.Framework.Services.System;
using System.Numerics;

namespace Neo.SmartContract
{
    public class AgencyTransaction : Framework.SmartContract
    {
        public static bool Main(byte[] agent, byte[] assetId, byte[] valueId, byte[] client, bool way, BigInteger price, byte[] signature)
        {
            if (VerifySignature(signature, client)) return true;
            if (!VerifySignature(signature, agent)) return false;
            byte[] inputId, outputId;
            if (way)
            {
                inputId = assetId;
                outputId = valueId;
            }
            else
            {
                inputId = valueId;
                outputId = assetId;
            }
            BigInteger inputSum = 0, outputSum = 0;
            TransactionOutput[] references = ((Transaction)ExecutionEngine.ScriptContainer).GetReferences();
            foreach (TransactionOutput reference in references)
            {
                if (reference.ScriptHash.Equals(ExecutionEngine.EntryScriptHash))
                {
                    if (!reference.AssetId.Equals(inputId))
                        return false;
                    else
                        inputSum += reference.Value;
                }
            }
            TransactionOutput[] outputs = ((Transaction)ExecutionEngine.ScriptContainer).GetOutputs();
            foreach (TransactionOutput output in outputs)
            {
                if (output.ScriptHash.Equals(ExecutionEngine.EntryScriptHash))
                {
                    if (output.AssetId.Equals(inputId))
                        inputSum -= output.Value;
                    else if (output.AssetId.Equals(outputId))
                        outputSum += output.Value;
                }
            }
            if (inputSum <= 0) return true;
            if (way)
            {
                if (outputSum * 100000000 < inputSum * price) return false;
            }
            else
            {
                if (inputSum * 100000000 > outputSum * price) return false;
            }
            return true;
        }
    }
}

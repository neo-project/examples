using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;

namespace Template.NEP17.CSharp
{
    public partial class NEP17 : SmartContract
    {
        private static bool ValidateAddress(UInt160 address) => address.IsValid && !address.IsZero;
        private static bool IsDeployed(UInt160 address) => ManagementContract.GetContract(address) != null;
    }
}

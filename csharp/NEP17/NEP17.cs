using Neo.SmartContract.Framework;
using System;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Examples
{
    [ManifestExtra("Author", "Neo")]
    [ManifestExtra("Email", "dev@neo.org")]
    [ManifestExtra("Description", "This is a NEP17 example")]
    [SupportedStandards("NEP-17")]
    [ContractPermission("*", "onNEP17Payment")]
    public partial class NEP17Demo : Framework.SmartContract
    {
        #region Token Settings
        [InitialValue("NhGobEnuWX5rVdpnuZZAZExPoRs5J6D2Sb", ContractParameterType.Hash160)]
        private static readonly UInt160 Owner = default;

        static readonly ulong MaxSupply = 10_000_000_000_000_000;
        static readonly ulong InitialSupply = 2_000_000_000_000_000;
        static readonly ulong TokensPerNEO = 1_000_000_000;
        static readonly ulong TokensPerGAS = 1;
        #endregion

        #region Notifications
        [DisplayName("Transfer")]
        public static event Action<UInt160, UInt160, BigInteger> OnTransfer;
        #endregion

        // When this contract address is included in the transaction signature,
        // this method will be triggered as a VerificationTrigger to verify that the signature is correct.
        // For example, this method needs to be called when withdrawing token from the contract.
        public static bool Verify() => IsOwner();

        [Safe]
        public static string Symbol() => "NTT";

        [Safe]
        public static ulong Decimals() => 8;

        [Safe]
        public static UInt160 GetOwner() => Owner;

    }
}

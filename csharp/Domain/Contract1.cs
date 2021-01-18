using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System;
using System.Numerics;

namespace Domain
{
    [ManifestExtra("Author", "Neo")]
    [ManifestExtra("Email", "dev@neo.org")]
    [ManifestExtra("Description", "This is a contract example")]
    public class Contract1 : SmartContract
    {
        public static UInt160 Query(string domain) => DomainStorage.Get(domain);

        public static bool Register(string domain, UInt160 owner)
        {
            if (!Runtime.CheckWitness(owner)) return false;
            if (DomainStorage.Get(domain) == UInt160.Zero) return false;
            DomainStorage.Put(domain, owner);
            return true;
        }

        public static bool Transfer(string domain, UInt160 to)
        {
            if (!Runtime.CheckWitness(DomainStorage.Get(domain))) return false;
            if (!Runtime.CheckWitness(to)) return false;
            DomainStorage.Put(domain, to);
            return true;
        }

        public static bool Delete(string domain)
        {
            if (!Runtime.CheckWitness(DomainStorage.Get(domain))) return false;
            DomainStorage.Delete(domain);
            return true;
        }
    }
}

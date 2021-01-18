using Neo;
using Neo.SmartContract.Framework.Services.Neo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageExample
{
    public static class OwnerStorage
    {
        public static readonly string mapName = "contract";

        public static readonly string key = "owner";

        public static void Put(UInt160 account) => Storage.CurrentContext.CreateMap(mapName).Put(key, account);

        public static UInt160 Get()
        {
            var value = Storage.CurrentContext.CreateMap(mapName).Get(key);
            return value.Length > 0 ? (UInt160)value : Contract1.InitialOwnerScriptHash;
        }
    }
}

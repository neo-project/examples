using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System.Numerics;
using Helper = Neo.SmartContract.Framework.Helper;

namespace StorageExample
{
    public static class BlacklistStorage
    {
        public static readonly string mapName = "blacklist";

        public static bool Initialize()
        {
            StorageMap storageMap = Storage.CurrentContext.CreateMap(mapName);
            var map = storageMap.Get(mapName);
            if (map != null) return false;
            storageMap.Put(mapName, Json.Serialize(new Map<UInt160, uint>()));
            return true;
        }

        public static void Add(UInt160 key, uint value)
        {
            StorageMap storageMap = Storage.CurrentContext.CreateMap(mapName);
            var map = Json.Deserialize(storageMap.Get(mapName)) as Map<UInt160, uint>;

            map[key] = value;

            storageMap.Put(mapName, Json.Serialize(map));
        }

        public static void Remove(UInt160 key)
        {
            StorageMap storageMap = Storage.CurrentContext.CreateMap(mapName);
            var map = Json.Deserialize(storageMap.Get(mapName)) as Map<UInt160, uint>;

            map.Remove(key);

            storageMap.Put(mapName, Json.Serialize(map));
        }

        public static bool Exist(UInt160 key)
        {
            StorageMap storageMap = Storage.CurrentContext.CreateMap(mapName);
            Map<UInt160, uint> map = Json.Deserialize(storageMap.Get(mapName)) as Map<UInt160, uint>;

            return map.HasKey(key);
        }
    }
}

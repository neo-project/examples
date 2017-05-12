using AntShares.SmartContract.Framework;
using AntShares.SmartContract.Framework.Services.AntShares;

namespace AntShares.SmartContract
{
    public class HelloWorld : FunctionCode
    {
        public static void Main()
        {
            byte[] key = { 0x48, 0x65, 0x6c, 0x6c, 0x6f }; // ASCII of "Hello"
            byte[] value = { 0x57, 0x6f, 0x72, 0x6c, 0x64 }; // ASCII of "World"
            Storage.Put(StorageContext.Current, key, value);
        }
    }
}

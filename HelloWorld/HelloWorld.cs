using AntShares.SmartContract.Framework;
using AntShares.SmartContract.Framework.Services.AntShares;

namespace AntShares.SmartContract
{
    public class HelloWorld : FunctionCode
    {
        public static void Main()
        {
            Storage.Put(StorageContext.Current, "Hello", "World");
        }
    }
}

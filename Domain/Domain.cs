using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;

namespace Neo.SmartContract
{
    public class Domain : FunctionCode
    {
        public static object Main(string operation, params object[] args)
        {
            Runtime.Log("Operation: " + operation);

            switch (operation)
            {
                case "query":
                    return Query((string)args[0]);
                case "register":
                    return Register((string)args[0], (byte[])args[1]);
                case "transfer":
                    return Transfer((string)args[0], (byte[])args[1]);
                case "delete":
                    return Delete((string)args[0]);
                default:
                    return false;
            }
        }

        private static byte[] Query(string domain)
        {
            byte[] value = Storage.Get(Storage.CurrentContext, domain);
            Runtime.Log("Query: " + domain + " : " + BytesToHex(value));
            return value;
        }

        private static bool Register(string domain, byte[] owner)
        {
            if (!Runtime.CheckWitness(owner))
            {
                Runtime.Log("Register: Failed Check Witness on Owner");
                return false;
            }
            byte[] value = Storage.Get(Storage.CurrentContext, domain);
            if (value != null)
            {
                Runtime.Log("Register: Failed as Owner already exists");
                return false;
            }
            Storage.Put(Storage.CurrentContext, domain, owner);
            Runtime.Log("Register: Successful for domain: " + domain);
            return true;
        }

        private static bool Transfer(string domain, byte[] to)
        {
            if (!Runtime.CheckWitness(to))
            {
                Runtime.Log("Transfer: Failed Check Witness on Owner");
                return false;
            }
            byte[] from = Storage.Get(Storage.CurrentContext, domain);
            if (from == null)
            {
                Runtime.Log("Transfer: Failed No domain owner");
                return false;
            }
            if (!Runtime.CheckWitness(from))
            {
                Runtime.Log("Transfer: Failed Check Witness on From");
                return false;
            }
            Storage.Put(Storage.CurrentContext, domain, to);
            Runtime.Log("Transfer: Succeeded");
            return true;
        }

        private static bool Delete(string domain)
        {
            byte[] owner = Storage.Get(Storage.CurrentContext, domain);
            if (owner == null)
            {
                Runtime.Log("Delete: Failed no Owner");
                return false;
            }
            if (!Runtime.CheckWitness(owner))
            {
                Runtime.Log("Delete: Failed Check Witness on Owner");
                return false;
            }
            Storage.Delete(Storage.CurrentContext, domain);
            Runtime.Log("Delete: Succeeded");
            return true;
        }

        private static string BytesToHex(byte[] bytes)
        {
            string hexString = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                hexString += bytes[i];
            }
            return hexString;
        }
    }
}

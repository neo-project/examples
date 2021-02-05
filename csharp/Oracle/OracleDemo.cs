using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System;

namespace Neo.SmartContract.Examples
{
    [ManifestExtra("Author", "Neo")]
    [ManifestExtra("Email", "dev@neo.org")]
    [ManifestExtra("Description", "This is an oracle example")]
    public class OracleDemo : Framework.SmartContract
    {
        public static void DoRequest()
        {
            string url = "https://github.com/neo-project/examples/tree/master/csharp/Oracle/example.json"; // the content is  { "value": "hello world" }
            string filter = "$.value";  // JSONPath format https://github.com/atifaziz/JSONPath
            string callback = "callback"; // callback method
            object userdata = "userdata"; // arbitrary type
            long gasForResponse = Oracle.MinimumResponseFee;

            Oracle.Request(url, filter, callback, userdata, gasForResponse);
        }

        public static void Callback(string url, string userdata, OracleResponseCode code, string result)
        {
            if (code != OracleResponseCode.Success) throw new Exception("Oracle response failure with code " + (byte)code);

            object ret = Json.Deserialize(result); // [ "hello world" ]
            object[] arr = (object[])ret;
            string value = (string)arr[0];

            Runtime.Log("userdata: " + userdata);
            Runtime.Log("response value: " + value);
        }
    }
}

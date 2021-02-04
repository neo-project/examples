using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using Neo.SmartContract.Framework.Services.System;
using System;
using System.ComponentModel;
using System.Numerics;

namespace OracleContract
{
    [DisplayName("Oracle Demo")]
    [ManifestExtra("Author", "Neo")]
    [ManifestExtra("Email", "dev@neo.org")]
    [ManifestExtra("Description", "This is an oracle example")]
    public class OracleDemo : SmartContract
    {
        public static void DoRequest()
        {
            string url = "https://127.0.0.1:8080/test"; // the return value is  { "value": "hello world" }, and when we use private host for testing, don't forget to set `AllowPrivateHost` true
            string filter = "$.value";  // JSONPath format https://github.com/atifaziz/JSONPath
            string callback = "callback"; // callback method
            object userdata = "userdata"; // arbitrary type
            long gasForResponse = Oracle.MinimumResponseFee;

            Oracle.Request(url, filter, callback, userdata, gasForResponse);
        }

        public static void Callback(string url, string userdata, OracleResponseCode code, string result)
        {
            if (code != OracleResponseCode.Success) throw new Exception(code.ToString());

            object ret = Json.Deserialize(result); // [ "hello world" ]
            object[] arr = (object[])ret;
            string value = (string)arr[0];

            Runtime.Log("userdata: " + userdata);
            Runtime.Log("response value: " + value);
        }
    }
}

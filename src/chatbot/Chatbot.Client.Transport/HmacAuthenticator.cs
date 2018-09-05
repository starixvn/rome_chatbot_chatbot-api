namespace Chatbot.Client.Transport
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;
    using RestSharp;
    using RestSharp.Authenticators;

    public class HmacAuthenticator : IAuthenticator
    {
        private readonly string machineIP;
        private readonly string secretKey;

        public HmacAuthenticator(string secretKey)
        {
            this.secretKey = secretKey;
            machineIP = GetLocalIPAddress();
        }

        public void Authenticate(IRestClient client, IRestRequest request)
        {
            var requestUri = HttpUtility.UrlEncode(client.BuildUri(request).OriginalString.ToLowerInvariant());
            string requestHttpMethod = request.Method.ToString();

            //Calculate UNIX time
            DateTime epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan timeSpan = DateTime.UtcNow - epochStart;
            string requestTimeStamp = Convert.ToUInt64(timeSpan.TotalSeconds).ToString();

            //create random nonce for each request
            string nonce = Guid.NewGuid().ToString("N");

            //Creating the raw signature string
            string signatureRawData = $"{machineIP}{requestHttpMethod}{requestUri}{requestTimeStamp}{nonce}";

            var secretKeyByteArray = Convert.FromBase64String(secretKey);

            byte[] signature = Encoding.UTF8.GetBytes(signatureRawData);

            using (var hmac = new HMACSHA256(secretKeyByteArray))
            {
                byte[] signatureBytes = hmac.ComputeHash(signature);
                string requestSignatureBase64String = Convert.ToBase64String(signatureBytes);

                // Setting the values in the Authorization header using custom scheme (amx)
                var authHeader = $"amx {machineIP}:{requestSignatureBase64String}:{nonce}:{requestTimeStamp}";
                request.AddParameter("Authorization", authHeader, ParameterType.HttpHeader);
            }
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString().Replace(":", "_");
                }
            }

            throw new InvalidOperationException("Local IP Address Not Found!");
        }
    }
}
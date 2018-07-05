namespace Alpha.Chatbot.WebApi.Filters
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Net.Http;
    using System.Runtime.Caching;
    using System.Security.Cryptography;
    using System.Security.Principal;
    using System.Text;
    using System.Threading;
    using System.Web;
    using Microsoft.AspNetCore.Authorization;

    /// <summary>
    /// http://bitoftech.net/2014/12/15/secure-asp-net-web-api-using-api-key-authentication-hmac-authentication/
    /// </summary>
    //[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public sealed class HmacAuthorizationFilter
    {
        private const long RequestMaxAgeInSeconds = 300;
        private const string TokenAuthenticationScheme = "amx"; 

        public void OnAuthorization(AuthorizationHandlerContext actionContext, HttpRequestMessage request)
        {
            var rawAuthzHeader = "request.Headers.Authorization.Parameter";
            var autherizationHeaderArray = GetAutherizationHeaderValues(rawAuthzHeader);

            if (autherizationHeaderArray != null)
            {
                var machineIP = autherizationHeaderArray[0];
                var incomingBase64Signature = autherizationHeaderArray[1];
                var nonce = autherizationHeaderArray[2];
                var requestTimeStamp = autherizationHeaderArray[3];

                var isValid = IsValidRequest(request, machineIP, incomingBase64Signature, nonce, requestTimeStamp);

                if (isValid)
                {
                    var currentPrincipal = new GenericPrincipal(new GenericIdentity(machineIP), null);
                    Thread.CurrentPrincipal = currentPrincipal;
                }
                else
                {
                }
            }
            else
            {
            }
            //if (request.Headers.Authorization != null && TokenAuthenticationScheme.Equals(request.Headers.Authorization.Scheme, StringComparison.OrdinalIgnoreCase))
            //{
            //}
            //else
            //{
            //    HandleUnauthorizedRequest(actionContext);
            //}
        }

        private string[] GetAutherizationHeaderValues(string rawAuthzHeader)
        {
            var credArray = rawAuthzHeader.Split(':');

            if (credArray.Length == 4)
            {
                return credArray;
            }
            else
            {
                return null;
            }
        }

        [SuppressMessage("Microsoft.Globalization",
            "CA1303:Do not pass literals as localized parameters",
            MessageId = "Fanex.Logging.LogService.Info(System.String)",
            Justification = "Reviewed")]
        private bool IsValidRequest(HttpRequestMessage req, string siteId, string incomingBase64Signature, string nonce, string requestTimeStamp)
        {
            string requestUri = HttpUtility.UrlEncode(req.RequestUri.AbsoluteUri.ToLower());
            string requestHttpMethod = req.Method.Method;

            var sharedKey = "share_key";

            if (IsReplayRequest(nonce, requestTimeStamp))
            {
                return false;
            }

            string data = string.Format("{0}{1}{2}{3}{4}", siteId, requestHttpMethod, requestUri, requestTimeStamp, nonce);

            var secretKeyBytes = Convert.FromBase64String(sharedKey);

            byte[] signature = Encoding.UTF8.GetBytes(data);

            using (HMACSHA256 hmac = new HMACSHA256(secretKeyBytes))
            {
                byte[] signatureBytes = hmac.ComputeHash(signature);

                return incomingBase64Signature.Equals(Convert.ToBase64String(signatureBytes), StringComparison.Ordinal);
            }
        }

        private bool IsReplayRequest(string nonce, string requestTimeStamp)
        {
            if (MemoryCache.Default.Contains(nonce))
            {
                return true;
            }

            var epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
            var currentTimeSpan = DateTime.UtcNow - epochStart;

            var serverTotalSeconds = Convert.ToUInt64(currentTimeSpan.TotalSeconds);
            var requestTotalSeconds = Convert.ToUInt64(requestTimeStamp);

            if (((long)serverTotalSeconds - (long)requestTotalSeconds) > RequestMaxAgeInSeconds)
            {
                return true;
            }

            MemoryCache.Default.Add(nonce, requestTimeStamp, DateTimeOffset.UtcNow.AddSeconds(RequestMaxAgeInSeconds));

            return false;
        }
    }
}
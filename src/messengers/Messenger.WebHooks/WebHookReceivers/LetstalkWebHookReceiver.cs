namespace Messenger.WebHooks.WebHookReceivers
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http.Controllers;
    using Microsoft.AspNet.WebHooks;

    public class LetstalkWebHookReceiver : WebHookReceiver
    {
        internal const string ReceiverName = "Letstalk";

        public override string Name => ReceiverName;

        public override async Task<HttpResponseMessage> ReceiveAsync(
            string id,
            HttpRequestContext context,
            HttpRequestMessage request)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.Method == HttpMethod.Post)
            {
                await VerifySignature(id, request);

                // Read the request entity body
                var data = await ReadAsJsonAsync(request);

                // TODO: Get the event actions, currently we dont have term action, I just init an empty enumerable
                var actions = Enumerable.Empty<string>();

                // Call registered handlers
                return await ExecuteWebHookAsync(id, context, request, actions, data);
            }

            /// TODO: Creates a response to a WebHook verification GET request, should implement this for testing alive webhook
            ////else if (request.Method == HttpMethod.Get)
            ////{
            ////    return await WebHookVerification(id, request);
            ////}

            return CreateBadMethodResponse(request);
        }

        protected virtual async Task VerifySignature(string id, HttpRequestMessage request)
        {
            //// TODO: Implement JWT Verifying Here
        }
    }
}
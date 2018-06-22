namespace Messenger.Client.Sample
{
    using System;
    using System.Collections.Generic;
    using Messenger.Client.Configurations;
    using Messenger.Client.Helpers;
    using Messenger.Client.Letstalk;
    using Messenger.Client.Models;
    using Newtonsoft.Json;
    using RestSharp;
    using RestSharp.Authenticators;

    public static class Program
    {
        private static void Main()
        {
            var restClient = new RestClient(AppSettings.MessengerApiEndpoint)
            {
                Authenticator = new JwtAuthenticator(AppSettings.MessengerApiKey)
            };

            var letstalkClient = new LetstalkClient(new RestClientHelper(restClient));

            Push(letstalkClient);

            PushToTag(letstalkClient);

            UpdateTag(letstalkClient);

            DeleteTag(letstalkClient);

            GetTags(letstalkClient);

            GetContacts(letstalkClient);

            AddContactToTag(letstalkClient);

            RemoveContactFromTag(letstalkClient);

            SetWebhook(letstalkClient);

            SetWelcomeMessage(letstalkClient);

            GetContactPublicKey(letstalkClient);

            SetContactKey(letstalkClient);

            SetHashKey(letstalkClient);

            Console.WriteLine("------------------ Async ------------------");

            PushAsync(letstalkClient);

            PushToTagAsync(letstalkClient);

            UpdateTagAsync(letstalkClient);

            DeleteTagAsync(letstalkClient);

            GetTagsAsync(letstalkClient);

            GetContactsAsync(letstalkClient);

            AddContactToTagAsync(letstalkClient);

            RemoveContactFromTagAsync(letstalkClient);

            SetWebhookAsync(letstalkClient);

            SetWelcomeMessageAsync(letstalkClient);

            GetContactPublicKeyAsync(letstalkClient);

            SetContactKeyAsync(letstalkClient);

            SetHashKeyAsync(letstalkClient);
        }

        private static void Push(LetstalkClient letstalkClient)
        {
            var result = letstalkClient.Push(
                            "botName",
                            new List<string>
                            {
                    "receiver_1",
                    "receiver_2"
                            },
                            new Payload
                            {
                                Id = 1,
                                From = 0,
                                UserId = "UserId",
                                HashContent = "Content"
                            });

            Console.WriteLine($"Request: Push, response: {JsonConvert.SerializeObject(result)}");
        }

        private static void SetWebhook(LetstalkClient letstalkClient)
        {
            var result = letstalkClient.SetWebhook(
                            "botName",
                            "webhook_url");

            Console.WriteLine($"Request: SetWebhook, response: {JsonConvert.SerializeObject(result)}");
        }

        private static void SetWelcomeMessage(LetstalkClient letstalkClient)
        {
            // API has not implemented;
            //var pushResult = letstalkClient.SetWelcomeMessage(
            //                "botName",
            //                "message");

            //Console.WriteLine(JsonConvert.SerializeObject(pushResult));
        }

        private static void PushToTag(LetstalkClient letstalkClient)
        {
            var result = letstalkClient.PushToTag(
                            "botName",
                            "tagName",
                            new Payload
                            {
                                Id = 1,
                                From = 0,
                                UserId = "UserId",
                                HashContent = "Content"
                            });

            Console.WriteLine($"Request: PushToTag, response: {JsonConvert.SerializeObject(result)}");
        }

        private static void UpdateTag(LetstalkClient letstalkClient)
        {
            Console.WriteLine($"Request: UpdateTag, response: need confirm");
        }

        private static void DeleteTag(LetstalkClient letstalkClient)
        {
            var result = letstalkClient.DeleteTag("botName", "tagName");

            Console.WriteLine($"Request: DeleteTag, response: {JsonConvert.SerializeObject(result)}");
        }

        private static void GetTags(LetstalkClient letstalkClient)
        {
            var result = letstalkClient.GetTags("botName");

            Console.WriteLine($"Request: GetTags, response: {JsonConvert.SerializeObject(result)}");
        }

        private static void GetContacts(LetstalkClient letstalkClient)
        {
            var result = letstalkClient.GetContacts("botName", "tag");

            Console.WriteLine($"Request: GetContacts, response: {JsonConvert.SerializeObject(result)}");
        }

        private static void AddContactToTag(LetstalkClient letstalkClient)
        {
            var result = letstalkClient.AddContactToTag("botName", "userid", "tag");

            Console.WriteLine($"Request: AddContactToTag, response: {JsonConvert.SerializeObject(result)}");
        }

        private static void RemoveContactFromTag(LetstalkClient letstalkClient)
        {
            var result = letstalkClient.RemoveContactFromTag("botName", "userid", "tag");

            Console.WriteLine($"Request: RemoveContactFromTag, response: {JsonConvert.SerializeObject(result)}");
        }

        private static void SetContactKey(LetstalkClient letstalkClient)
        {
            var result = letstalkClient.SetContactKey("botName", "userid", "hash_id", "hash_key");

            Console.WriteLine($"Request: SetContactKey, response: {JsonConvert.SerializeObject(result)}");
        }

        private static void GetContactPublicKey(LetstalkClient letstalkClient)
        {
            var result = letstalkClient.GetContactPublicKey("botName", "userid");

            Console.WriteLine($"Request: GetContactPublicKey, response: {JsonConvert.SerializeObject(result)}");
        }

        private static void SetHashKey(LetstalkClient letstalkClient)
        {
            var result = letstalkClient.SetHashKey("botName", "hash_id", "hash_key");

            Console.WriteLine($"Request: SetHashKey, response: {JsonConvert.SerializeObject(result)}");
        }

        // Async
        private static void PushAsync(LetstalkClient letstalkClient)
        {
            var result = letstalkClient.PushAsync(
                            "botName",
                            new List<string>
                            {
                    "receiver_1",
                    "receiver_2"
                            },
                            new Payload
                            {
                                Id = 1,
                                From = 0,
                                UserId = "UserId",
                                HashContent = "Content"
                            }).Result;

            Console.WriteLine($"Request: PushAsync, response: {JsonConvert.SerializeObject(result)}");
        }

        private static void SetWebhookAsync(LetstalkClient letstalkClient)
        {
            var result = letstalkClient.SetWebhookAsync(
                            "botName",
                            "webhook_url").Result;

            Console.WriteLine($"Request: SetWebhookAsync, response: {JsonConvert.SerializeObject(result)}");
        }

        private static void SetWelcomeMessageAsync(LetstalkClient letstalkClient)
        {
            // API has not implemented;
            //var pushResult = letstalkClient.SetWelcomeMessage(
            //                "botName",
            //                "message");

            //Console.WriteLine(JsonConvert.SerializeObject(pushResult));
        }

        private static void PushToTagAsync(LetstalkClient letstalkClient)
        {
            var result = letstalkClient.PushToTagAsync(
                            "botName",
                            "tagName",
                            new Payload
                            {
                                Id = 1,
                                From = 0,
                                UserId = "UserId",
                                HashContent = "Content"
                            }).Result;

            Console.WriteLine($"Request: PushToTagAsync, response: {JsonConvert.SerializeObject(result)}");
        }

        private static void UpdateTagAsync(LetstalkClient letstalkClient)
        {
            Console.WriteLine($"Request: UpdateTagAsync, response: need confirm");
        }

        private static void DeleteTagAsync(LetstalkClient letstalkClient)
        {
            var result = letstalkClient.DeleteTagAsync("botName", "tagName").Result;

            Console.WriteLine($"Request: DeleteTagAsync, response: {JsonConvert.SerializeObject(result)}");
        }

        private static void GetTagsAsync(LetstalkClient letstalkClient)
        {
            var result = letstalkClient.GetTagsAsync("botName").Result;

            Console.WriteLine($"Request: GetTagsAsync, response: {JsonConvert.SerializeObject(result)}");
        }

        private static void GetContactsAsync(LetstalkClient letstalkClient)
        {
            var result = letstalkClient.GetContactsAsync("botName", "tag").Result;

            Console.WriteLine($"Request: GetContactsAsync, response: {JsonConvert.SerializeObject(result)}");
        }

        private static void AddContactToTagAsync(LetstalkClient letstalkClient)
        {
            var result = letstalkClient.AddContactToTagAsync("botName", "userid", "tag").Result;

            Console.WriteLine($"Request: AddContactToTagAsync, response: {JsonConvert.SerializeObject(result)}");
        }

        private static void RemoveContactFromTagAsync(LetstalkClient letstalkClient)
        {
            var result = letstalkClient.RemoveContactFromTagAsync("botName", "userid", "tag").Result;

            Console.WriteLine($"Request: RemoveContactFromTagAsync, response: {JsonConvert.SerializeObject(result)}");
        }

        private static void SetContactKeyAsync(LetstalkClient letstalkClient)
        {
            var result = letstalkClient.SetContactKeyAsync("botName", "userid", "hash_id", "hash_key").Result;

            Console.WriteLine($"Request: SetContactKeyAsync, response: {JsonConvert.SerializeObject(result)}");
        }

        private static void GetContactPublicKeyAsync(LetstalkClient letstalkClient)
        {
            var result = letstalkClient.GetContactPublicKeyAsync("botName", "userid").Result;

            Console.WriteLine($"Request: GetContactPublicKeyAsync, response: {JsonConvert.SerializeObject(result)}");
        }

        private static void SetHashKeyAsync(LetstalkClient letstalkClient)
        {
            var result = letstalkClient.SetHashKeyAsync("botName", "hash_id", "hash_key").Result;

            Console.WriteLine($"Request: SetHashKeyAsync, response: {JsonConvert.SerializeObject(result)}");
        }
    }
}
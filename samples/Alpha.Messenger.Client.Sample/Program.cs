namespace Alpha.Messenger.Client.Sample
{
    using System;
    using System.Collections.Generic;
    using Alpha.Messenger.Client.Helpers;
    using Alpha.Messenger.Client.Messengers;
    using Alpha.Messenger.Client.Models;
    using Newtonsoft.Json;

    public static class Program
    {
        private static void Main()
        {
            var letstalkClient = new LetstalkClient(new RestClientHelper());

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
    }
}
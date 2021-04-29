using System;
using System.Linq;
using System.Threading.Tasks;
using Google.Api.Gax;
using Google.Api.Gax.Grpc;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.PubSub.V1;
using Grpc.Auth;
using Grpc.Core;

namespace LeadGuru.Ingress
{
    class Program
    {
        // GCP ProjectID
        private const string ProjectId = "lead-tool-generator";
        // Pubsub topic name
        private const string TopicId = "prod-aggregator_publish";

        static async Task Main(string[] args)
        {
            // has to be a constant like projectId and topicId.
            // for simplicity we use machine name here as en example
            var formattedMachineName = new string(System.Net.Dns.GetHostName().Where(char.IsLetterOrDigit).ToArray()); 
            string subscriptionId = $"subscriber_{formattedMachineName}";
            
            
            var subscriptionName = new SubscriptionName(ProjectId, subscriptionId);
            var topicName = new TopicName(ProjectId, TopicId);

            // let's make sure that subscription exists and is not expired
            await CreateSubsriptionIfDoesntExist(subscriptionName, topicName);
            var client = await CreateSubscribeClient(subscriptionName);

            while (true)
            {
                await client.StartAsync((message, cancel) =>
                {
                    string text = System.Text.Encoding.UTF8.GetString(message.Data.ToArray());
                    
                    // Print message here
                    Console.WriteLine(text);
                    
                    return Task.FromResult(SubscriberClient.Reply.Ack);
                });
            }
        }
        
        private static async Task<SubscriberClient> CreateSubscribeClient(SubscriptionName subscriptionName)
        {
            var credential = GoogleCredential.FromFile(Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS"));
            var subscribeSettings = new SubscriberClient.ClientCreationSettings(credentials: credential.ToChannelCredentials());
            return await SubscriberClient.CreateAsync(subscriptionName, subscribeSettings);
        }
        
        private static async Task CreateSubsriptionIfDoesntExist(SubscriptionName subscribtionName, TopicName topicName)
        {
            var subscriberService = await SubscriberServiceApiClient.CreateAsync();
            try
            {
                subscriberService.GetSubscription(subscribtionName);
            }
            catch (RpcException rex)
            {
                if (rex.Status.StatusCode == StatusCode.NotFound)
                {
                    // expire subscription if it's not in use
                    var callSettings = Expiration.FromTimeout(TimeSpan.FromHours(1));
                    subscriberService.CreateSubscription(subscribtionName, topicName, pushConfig: null, ackDeadlineSeconds: 60, callSettings: CallSettings.FromExpiration(callSettings));
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
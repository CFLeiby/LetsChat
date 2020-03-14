using LetsChat.Services.DataAccess;
using LetsChat.Services.Hubs;
using LetsChat.Services.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LetsChat.Data
{
    /// <summary>
    /// Implements the necessary persistence layer methods required by our services/
    /// business layer.  Barring a real persistence source (i.e. db), this currently
    /// returns predefined sample data
    /// </summary>
    public class MessageRepository : IMessageRepository
    {
        private const string sbConnectString = "ServiceBusConnectionString";
        private readonly IQueueClient queueClient;
        private readonly IHubContext<ChatHub> _hubContext;

        public MessageRepository(IConfiguration config, IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;

            var connectionString = config[sbConnectString];
            queueClient = new QueueClient(connectionString, "letschat", ReceiveMode.PeekLock, RetryPolicy.Default);

            // Configure message handler options 
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };

            // Register function to processes messages.
            queueClient.RegisterMessageHandler(SendMessageToClients, messageHandlerOptions);
        }

        /// <summary>
        /// Takes the supplied message and saves it off to the appropriate topic
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async Task SaveMessage(ChatMessage msg)
        {
            try
            {
                // Turn the msg into a json string
                var json = JsonConvert.SerializeObject(msg);

                // Log the message to the console.
                Console.WriteLine($"Sending message: {json}");

                //Wrap it in a Message
                var message = new Message(Encoding.UTF8.GetBytes(json));

                // And send the message to the queue.
                await queueClient.SendAsync(message);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
            }
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }

        private async Task SendMessageToClients(Message message, CancellationToken token)
        {
            try
            {
                // Process the message.
                var body = Encoding.UTF8.GetString(message.Body);
                Console.WriteLine($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{body}");

                var msg = JsonConvert.DeserializeObject<ChatMessage>(body);

                await _hubContext.Clients.Group(msg.ChatRoom)?.SendAsync("ReceiveMessage", msg);

                // Mark the message complete
                await queueClient.CompleteAsync(message.SystemProperties.LockToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to process message: SequenceNumber:{message.SystemProperties.SequenceNumber} Exception: {ex}");
            }
        }
    }
}

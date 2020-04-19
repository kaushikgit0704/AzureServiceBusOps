using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;

namespace QueueReceive
{
    class Program
    {
        static string connStr = "Endpoint=sb://demosbnamespace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SDB48GAruIN6R/PRmd9AOmVGmUIyoThC86c5nezRHco=";
        static string queueName = "sbqueue";
        static IQueueClient queueClient;
        static void Main(string[] args)
        {
            Receive().GetAwaiter().GetResult();
        }

        static async Task Receive()
        {
            var manager = new ManagementClient(connStr);
            queueClient = new QueueClient(connStr, queueName);
            try
            {
                if (!await manager.QueueExistsAsync(queueName))
                {
                    Console.WriteLine("No Queue Exists !");
                }
                else
                {                    
                    MessageHandlerOptions options = new MessageHandlerOptions(ExceptionReceivedHandler)
                    {
                        AutoComplete = false,
                        MaxConcurrentCalls = 1
                    };
                    queueClient.RegisterMessageHandler(ProcessMessage, options);

                    Console.ReadKey();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something Wrong: " + ex.Message);
            }
            finally 
            {
                await queueClient.CloseAsync();
            }
            
        }

        private static async Task ProcessMessage(Message message, CancellationToken cToken)
        {
            Console.WriteLine("Received: " + Encoding.UTF8.GetString(message.Body));
            await queueClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        private static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs arg)
        {
            Console.WriteLine("Exception received: " + arg.Exception);
            return Task.CompletedTask;
        }

       
    }
}

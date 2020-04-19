using System;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using System.Threading.Tasks;
using QueueManager;
using System.Text;

namespace QueueSend
{
    class Program
    {
        static string connStr = "Endpoint=sb://demosbnamespace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SDB48GAruIN6R/PRmd9AOmVGmUIyoThC86c5nezRHco=";
        static string queueName = "sbqueue";
        static void Main(string[] args)
        {
            SendToQueue().GetAwaiter().GetResult();
        }

        static async Task SendToQueue()
        {
            var manager = new ManagementClient(connStr);
            if (!await manager.QueueExistsAsync(queueName))
            {
                var queueManager = new ManageQueue(connStr, queueName);
                await queueManager.CreateQueue();
            }

            IQueueClient queueClient = new QueueClient(connStr,queueName);
            try
            {
                for (int count = 0; count < 10; count++)
                {
                    var messageBody = "Message " + (count + 1);
                    var message = new Message(Encoding.UTF8.GetBytes(messageBody));
                    Console.WriteLine("Sending Message: " + messageBody);
                    await queueClient.SendAsync(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something wrong happened: " + ex.Message);
            }
            finally
            {
                await queueClient.CloseAsync();
            }                        
        }
    }
}

using System;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;

namespace QueueManager
{
    public class ManageQueue
    {
        private string connStr;
        private string queueName;

        public ManageQueue(string connStr, string queueName)
        {
            this.connStr = connStr;
            this.queueName = queueName;
        }

        public async Task CreateQueue()
        {
            var manager = new ManagementClient(this.connStr);
            if (!await manager.QueueExistsAsync(this.queueName))
            {
                await manager.CreateQueueAsync(this.queueName);
            }
            else
            {
                Console.WriteLine("Queue Exists");
            }
        }
    }
}

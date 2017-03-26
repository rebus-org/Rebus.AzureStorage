using System;
using Microsoft.WindowsAzure.Storage;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Routing.TypeBased;
#pragma warning disable 1998

namespace SmallDemo
{
    class Program
    {
        static void Main()
        {
            var storageAccount = CloudStorageAccount.DevelopmentStorageAccount;

            using (var clientActivator = new BuiltinHandlerActivator())
            {
                var client = Configure.With(clientActivator)
                    .Transport(t => t.UseAzureStorageQueuesAsOneWayClient(storageAccount))
                    .Routing(r => r.TypeBased().Map<string>("server"))
                    .Start();

                using (var serverActivator = new BuiltinHandlerActivator())
                {
                    serverActivator.Handle<string>(async message => Console.WriteLine($"Got message: {message}"));

                    Configure.With(serverActivator)
                        .Transport(t => t.UseAzureStorageQueues(storageAccount, "server"))
                        .Start();

                    client.Advanced.SyncBus.Send("HEJ MED DIG MIN VEN!!!");

                    Console.WriteLine("Press ENTER to quit");
                    Console.ReadLine();
                }
            }
        }
    }
}

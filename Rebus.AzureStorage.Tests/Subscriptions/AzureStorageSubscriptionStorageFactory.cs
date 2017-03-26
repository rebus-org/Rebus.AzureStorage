using System;
using Rebus.AzureStorage.Subscriptions;
using Rebus.Logging;
using Rebus.Subscriptions;
using Rebus.Tests.Contracts.Subscriptions;

namespace Rebus.AzureStorage.Tests.Subscriptions
{
    public class AzureStorageSubscriptionStorageFactory : AzureStorageFactoryBase, ISubscriptionStorageFactory
    {
        static readonly string TableName = $"RebusSubscriptionsTest{DateTime.Now:yyyyMMddHHmmss}";

        public ISubscriptionStorage Create()
        {
            var subscriptionStorage = new AzureStorageSubscriptionStorage(StorageAccount, new ConsoleLoggerFactory(false), false, TableName);
            subscriptionStorage.Initialize();
            return subscriptionStorage;
        }

        public void Cleanup()
        {
            DropTable(TableName);
        }
    }
}

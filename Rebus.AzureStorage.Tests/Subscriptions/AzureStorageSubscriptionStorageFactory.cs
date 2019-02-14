//using System;
//using Rebus.AzureStorage.Subscriptions;
//using Rebus.Logging;
//using Rebus.Subscriptions;
//using Rebus.Tests.Contracts.Subscriptions;

//namespace Rebus.AzureStorage.Tests.Subscriptions
//{
//    public class AzureStorageSubscriptionStorageFactory : AzureStorageFactoryBase, ISubscriptionStorageFactory
//    {
//        static readonly Random Random = new Random(DateTime.Now.GetHashCode());

//        readonly string _tableName = $"rebussubtest{DateTime.Now:yyyyMMddHHmmss}{Random.Next(1000):0000}";

//        public ISubscriptionStorage Create()
//        {
//            var subscriptionStorage = new AzureStorageSubscriptionStorage(StorageAccount, new ConsoleLoggerFactory(false), false, _tableName);
//            subscriptionStorage.Initialize();
//            return subscriptionStorage;
//        }

//        public void Cleanup()
//        {
//            DropTable(_tableName);
//        }
//    }
//}

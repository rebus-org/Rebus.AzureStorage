using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Microsoft.WindowsAzure.Storage.Table;
using Rebus.AzureStorage.Entities;
using Rebus.Exceptions;
using Rebus.Logging;
using Rebus.Subscriptions;

namespace Rebus.AzureStorage.Subscriptions
{
    /// <summary>
    /// Implementation of <see cref="ISubscriptionStorage"/> that uses table storage to store subscriptions
    /// </summary>
    public class AzureStorageSubscriptionStorage : ISubscriptionStorage
    {
        readonly CloudStorageAccount _cloudStorageAccount;
        readonly IRebusLoggerFactory _loggerFactory;
        readonly string _tableName;

        /// <summary>
        /// Creates the subscription storage
        /// </summary>
        public AzureStorageSubscriptionStorage(CloudStorageAccount cloudStorageAccount,
            IRebusLoggerFactory loggerFactory,
            bool isCentralized = false,
            string tableName = "RebusSubscriptions")
        {
            IsCentralized = isCentralized;
            _cloudStorageAccount = cloudStorageAccount;
            _loggerFactory = loggerFactory;
            _tableName = tableName;
        }

        public void EnsureCreated()
        {
            _loggerFactory.GetLogger<AzureStorageSubscriptionStorage>().Info("Auto creating table {0}", _tableName);
            var client = _cloudStorageAccount.CreateCloudTableClient();
            var tableReference = client.GetTableReference(_tableName);
            AsyncHelpers.RunSync(() => tableReference.CreateIfNotExistsAsync());
        }

        // PartitionKey = Topic
        // RowKey = Address

        public async Task<string[]> GetSubscriberAddresses(string topic)
        {
            try
            {
                var condition = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, topic);
                var query = new TableQuery<AzureStorageSubscription>().Where(condition);
                var table = GetTable();
                var operationContext = new OperationContext();
                var tableRequestOptions = new TableRequestOptions { RetryPolicy = new ExponentialRetry() };
                var items = await table.ExecuteQueryAsync(query, tableRequestOptions, operationContext);
                return items.Select(i => i.RowKey).ToArray();
            }
            catch (StorageException exception)
            {
                throw new RebusApplicationException(exception, $"Could not get subscriber addresses for '{topic}'");
            }
        }

        public async Task RegisterSubscriber(string topic, string subscriberAddress)
        {
            try
            {
                var entity = new AzureStorageSubscription(topic, subscriberAddress);
                var table = GetTable();
                var operationContext = new OperationContext();
                var tableRequestOptions = new TableRequestOptions { RetryPolicy = new ExponentialRetry() };
                var result = await table.ExecuteAsync(TableOperation.InsertOrReplace(entity), tableRequestOptions, operationContext);
            }
            catch (Exception exception)
            {
                throw new RebusApplicationException(exception, $"Could not subscribe {subscriberAddress} to '{topic}'");
            }
        }

        public async Task UnregisterSubscriber(string topic, string subscriberAddress)
        {
            try
            {
                var entity = new AzureStorageSubscription(topic, subscriberAddress) { ETag = "*" };
                var tableReference = GetTable();
                var operationContext = new OperationContext();
                var result = await tableReference.ExecuteAsync(TableOperation.Delete(entity), new TableRequestOptions { RetryPolicy = new ExponentialRetry() }, operationContext);
            }
            catch (Exception exception)
            {
                throw new RebusApplicationException(exception, $"Could not unsubscribe {subscriberAddress} from '{topic}'");
            }
        }

        /// <summary>
        /// Gets whether this subscription storage is centralized (i.e. whether subscribers can register themselves directly)
        /// </summary>
        public bool IsCentralized { get; }

        public void DropTables()
        {
            var client = _cloudStorageAccount.CreateCloudTableClient();
            var tableReference = client.GetTableReference(_tableName);
            AsyncHelpers.RunSync(() => tableReference.DeleteIfExistsAsync());
        }

        CloudTable GetTable()
        {
            var client = _cloudStorageAccount.CreateCloudTableClient();
            return client.GetTableReference(_tableName);
        }
    }
}

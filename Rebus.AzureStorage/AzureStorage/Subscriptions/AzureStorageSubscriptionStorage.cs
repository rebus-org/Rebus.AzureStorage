using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Microsoft.WindowsAzure.Storage.Table;
using Rebus.Bus;
using Rebus.Exceptions;
using Rebus.Logging;
using Rebus.Subscriptions;

namespace Rebus.AzureStorage.Subscriptions
{
    /// <summary>
    /// Implementation of <see cref="ISubscriptionStorage"/> that uses table storage to store subscriptions
    /// </summary>
    public class AzureStorageSubscriptionStorage : ISubscriptionStorage, IInitializable
    {
        readonly CloudStorageAccount _cloudStorageAccount;
        readonly string _tableName;
        readonly ILog _log;

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
            _log = loggerFactory.GetLogger<AzureStorageSubscriptionStorage>();
            _tableName = tableName;
        }

        /// <summary>
        /// Initializes the subscription storage by ensuring that the necessary table is created
        /// </summary>
        public void Initialize()
        {
            _log.Info("Auto creating table {0}", _tableName);
            var client = _cloudStorageAccount.CreateCloudTableClient();
            var tableReference = client.GetTableReference(_tableName);
            AsyncHelpers.RunSync(() => tableReference.CreateIfNotExistsAsync());
        }

        /// <summary>
        /// Gets all subscribers by getting row IDs from the partition named after the given <paramref name="topic"/>
        /// </summary>
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

        /// <summary>
        /// Registers the given <paramref name="subscriberAddress"/> as a subscriber of the topic named <paramref name="topic"/>
        /// by inserting a row with the address as the row ID under a partition key named after the topic
        /// </summary>
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

        /// <summary>
        /// Unregisters the given <paramref name="subscriberAddress"/> as a subscriber of the topic named <paramref name="topic"/>
        /// by removing the row with the address as the row ID under a partition key named after the topic
        /// </summary>
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

        /// <summary>
        /// Drops the table used by the subscription storage
        /// </summary>
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

using System;
using Microsoft.WindowsAzure.Storage;
using Rebus.Auditing.Sagas;
using Rebus.AzureStorage.Sagas;
using Rebus.Logging;
// ReSharper disable UnusedMember.Global

namespace Rebus.Config
{
    /// <summary>
    /// Configuration extensions for Azure storage
    /// </summary>
    public static class AzureStorageSagaConfigurationExtensions
    {
        /// <summary>
        /// Configures Rebus to store saga data snapshots in blob storage
        /// </summary>
        public static void StoreInBlobStorage(this StandardConfigurer<ISagaSnapshotStorage> configurer, CloudStorageAccount cloudStorageAccount, string containerName = "RebusSagaStorage")
        {
            if (configurer == null) throw new ArgumentNullException(nameof(configurer));
            if (cloudStorageAccount == null) throw new ArgumentNullException(nameof(cloudStorageAccount));
            if (containerName == null) throw new ArgumentNullException(nameof(containerName));

            configurer.Register(c => new AzureStorageSagaSnapshotStorage(cloudStorageAccount, c.Get<IRebusLoggerFactory>(), containerName));
        }

        /// <summary>
        /// Configures Rebus to store saga data snapshots in blob storage
        /// </summary>
        public static void StoreInBlobStorage(this StandardConfigurer<ISagaSnapshotStorage> configurer, string storageAccountConnectionString, string containerName = "RebusSagaStorage")
        {
            if (configurer == null) throw new ArgumentNullException(nameof(configurer));
            if (storageAccountConnectionString == null) throw new ArgumentNullException(nameof(storageAccountConnectionString));
            if (containerName == null) throw new ArgumentNullException(nameof(containerName));

            var storageAccount = CloudStorageAccount.Parse(storageAccountConnectionString);

            configurer.Register(c => new AzureStorageSagaSnapshotStorage(storageAccount, c.Get<IRebusLoggerFactory>(), containerName));
        }
    }
}
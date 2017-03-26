using System;
using Microsoft.WindowsAzure.Storage;
using Rebus.Auditing.Sagas;
using Rebus.AzureStorage.Sagas;
using Rebus.Config;
using Rebus.Logging;

namespace Rebus.AzureStorage.Config
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
    }
}
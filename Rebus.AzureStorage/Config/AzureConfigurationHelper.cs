using System;
using Microsoft.WindowsAzure.Storage;

namespace Rebus.Config
{
    class AzureConfigurationHelper
    {
        public static CloudStorageAccount GetStorageAccount(string storageAccountConnectionStringOrName)
        {
            if (storageAccountConnectionStringOrName == null)
            {
                throw new ArgumentNullException(nameof(storageAccountConnectionStringOrName));
            }

#if NET45
            var isConnectionString = storageAccountConnectionStringOrName.ToLowerInvariant().Replace(" ", "").Contains("accountkey=");

            if (!isConnectionString)
            {
                var connectionStringSettings = System.Configuration.ConfigurationManager.ConnectionStrings[storageAccountConnectionStringOrName];
                if (connectionStringSettings == null)
                {
                    throw new Exceptions.RebusConfigurationException($"Could not find connection string named '{storageAccountConnectionStringOrName}' in the current application configuration file");
                }
                storageAccountConnectionStringOrName = connectionStringSettings.ConnectionString;
            }
#endif

            var storageAccount = CloudStorageAccount.Parse(storageAccountConnectionStringOrName);
            return storageAccount;
        }
    }
}
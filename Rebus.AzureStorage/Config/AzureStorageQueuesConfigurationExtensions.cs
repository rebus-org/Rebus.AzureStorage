using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Rebus.AzureStorage;
using Rebus.AzureStorage.Transport;
using Rebus.Logging;
using Rebus.Pipeline;
using Rebus.Pipeline.Receive;
using Rebus.Timeouts;
using Rebus.Transport;
// ReSharper disable ArgumentsStyleNamedExpression

namespace Rebus.Config
{
    /// <summary>
    /// Configuration extensions for the Aure Storage Queue transport
    /// </summary>
    public static class AzureStorageQueuesConfigurationExtensions
    {
        const string AsqTimeoutManagerText = @"A disabled timeout manager was installed as part of the Azure Storage Queues configuration, becuase the transport has native support for deferred messages.

If you don't want to use Azure Storage Queues' native support for deferred messages, please pass AzureStorageQueuesTransportOptions with UseNativeDeferredMessages = false when
configuring the transport, e.g. like so:

Configure.With(...)
    .Transport(t => {
        var options = new AzureStorageQueuesTransportOptions { UseNativeDeferredMessages = false };

        t.UseAzureStorageQueues(storageAccount, ""my-queue"", options: options);
    })
    .(...)
    .Start();";

        /// <summary>
        /// Configures Rebus to use Azure Storage Queues to transport messages as a one-way client (i.e. will not be able to receive any messages)
        /// </summary>
        public static void UseAzureStorageQueuesAsOneWayClient(this StandardConfigurer<ITransport> configurer, string storageAccountConnectionStringOrName, AzureStorageQueuesTransportOptions options = null)
        {
            var storageAccount = AzureConfigurationHelper.GetStorageAccount(storageAccountConnectionStringOrName);

            Register(configurer, null, storageAccount, options);

            OneWayClientBackdoor.ConfigureOneWayClient(configurer);
        }

        /// <summary>
        /// Configures Rebus to use Azure Storage Queues to transport messages
        /// </summary>
        public static void UseAzureStorageQueues(this StandardConfigurer<ITransport> configurer, string storageAccountConnectionStringOrName, string inputQueueAddress, AzureStorageQueuesTransportOptions options = null)
        {
            var storageAccount = AzureConfigurationHelper.GetStorageAccount(storageAccountConnectionStringOrName);

            Register(configurer, inputQueueAddress, storageAccount, options);
        }

        /// <summary>
        /// Configures Rebus to use Azure Storage Queues to transport messages as a one-way client (i.e. will not be able to receive any messages)
        /// </summary>
        public static void UseAzureStorageQueuesAsOneWayClient(this StandardConfigurer<ITransport> configurer, string accountName, string keyValue, bool useHttps, AzureStorageQueuesTransportOptions options = null)
        {
            var storageAccount = new CloudStorageAccount(new StorageCredentials(accountName, keyValue), useHttps);

            Register(configurer, null, storageAccount, options);

            OneWayClientBackdoor.ConfigureOneWayClient(configurer);
        }

        /// <summary>
        /// Configures Rebus to use Azure Storage Queues to transport messages
        /// </summary>
        public static void UseAzureStorageQueues(this StandardConfigurer<ITransport> configurer, string accountName, string keyValue, bool useHttps, string inputQueueAddress, AzureStorageQueuesTransportOptions options = null)
        {
            var storageAccount = new CloudStorageAccount(new StorageCredentials(accountName, keyValue), useHttps);

            Register(configurer, inputQueueAddress, storageAccount, options);
        }

        /// <summary>
        /// Configures Rebus to use Azure Storage Queues to transport messages as a one-way client (i.e. will not be able to receive any messages)
        /// </summary>
        public static void UseAzureStorageQueuesAsOneWayClient(this StandardConfigurer<ITransport> configurer, CloudStorageAccount storageAccount, AzureStorageQueuesTransportOptions options = null)
        {
            Register(configurer, null, storageAccount, options);

            OneWayClientBackdoor.ConfigureOneWayClient(configurer);
        }

        /// <summary>
        /// Configures Rebus to use Azure Storage Queues to transport messages
        /// </summary>
        public static void UseAzureStorageQueues(this StandardConfigurer<ITransport> configurer, CloudStorageAccount storageAccount, string inputQueueAddress, AzureStorageQueuesTransportOptions options = null)
        {
            Register(configurer, inputQueueAddress, storageAccount, options);
        }

        static void Register(StandardConfigurer<ITransport> configurer, string inputQueueAddress, CloudStorageAccount storageAccount, AzureStorageQueuesTransportOptions optionsOrNull)
        {
            if (configurer == null) throw new ArgumentNullException(nameof(configurer));
            if (storageAccount == null) throw new ArgumentNullException(nameof(storageAccount));

            var options = optionsOrNull ?? new AzureStorageQueuesTransportOptions();

            configurer.Register(c =>
            {
                var rebusLoggerFactory = c.Get<IRebusLoggerFactory>();
                return new AzureStorageQueuesTransport(storageAccount, inputQueueAddress, rebusLoggerFactory, options);
            });

            if (options.UseNativeDeferredMessages)
            {
                configurer.OtherService<ITimeoutManager>().Register(c => new DisabledTimeoutManager(), description: AsqTimeoutManagerText);

                configurer.OtherService<IPipeline>().Decorate(c =>
                {
                    var pipeline = c.Get<IPipeline>();

                    return new PipelineStepRemover(pipeline)
                        .RemoveIncomingStep(s => s.GetType() == typeof(HandleDeferredMessagesStep));
                });
            }
        }
    }
}
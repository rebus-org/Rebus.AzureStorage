using Rebus.Bus;

namespace Rebus.Config
{
    /// <summary>
    /// Options to configure behavior of Rebus' Azure Storage Queues transport
    /// </summary>
    public class AzureStorageQueuesTransportOptions
    {
        /// <summary>
        /// Configures whether Azure Storage Queues' built-in support for deferred messages should be used.
        /// Defaults to <code>true</code>. When set to <code>false</code>, please remember to register
        /// a timeout manager, or configure another endpoint as a timeout manager, if you intend to
        /// <see cref="IBus.Defer"/> or <see cref="IBus.DeferLocal"/> messages.
        /// </summary>
        public bool UseNativeDeferredMessages { get; set; } = true;

        /// <summary>
        /// Configures how many messages to prefetch. Valid values are null, 0, ... 32
        /// </summary>
        public int? Prefetch { get; set; } = null;
    }
}
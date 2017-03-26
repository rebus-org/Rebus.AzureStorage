using NUnit.Framework;
using Rebus.Tests.Contracts.Subscriptions;

namespace Rebus.AzureStorage.Tests.Subscriptions
{
    public class AzureSubscriptionStorageBasicSubscriptionOperations : BasicSubscriptionOperations<AzureStorageSubscriptionStorageFactory>
    {
        [OneTimeSetUp]
        public void FixtureSetup()
        {
            AzureStorageSubscriptionStorageFactory.CreateTables();
        }

        [OneTimeTearDown]
        public void FixtureTeardown()
        {
            AzureStorageSubscriptionStorageFactory.DropTables();
        }
    }
}
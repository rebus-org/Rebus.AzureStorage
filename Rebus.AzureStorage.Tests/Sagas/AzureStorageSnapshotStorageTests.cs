using NUnit.Framework;
using Rebus.Tests.Contracts.Sagas;

namespace Rebus.AzureStorage.Tests.Sagas
{
    [TestFixture]
    public class AzureStorageSnapshotStorageTests : SagaSnapshotStorageTest<AzureStorageSagaSnapshotStorageFactory>
    {
    }
}
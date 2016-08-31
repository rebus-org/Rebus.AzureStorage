using Rebus.AzureStorage.Sagas;
using Rebus.Logging;
using Rebus.Sagas;
using Rebus.Tests.Contracts.Sagas;

namespace Rebus.AzureStorage.Tests.Sagas
{
    public class AzureStorageSagaStorageFactory : AzureStorageFactoryBase, ISagaStorageFactory//, ISagaSnapshotStorageFactory
    {
        public static readonly string ContainerName = $"NewRebusSagaStorageTestContainer";
        public static readonly string TableName = $"NewRebusSagaStorageTestTable";

        public ISagaStorage GetSagaStorage()
        {
            lock (ContainerName)
            {
                var storage = new AzureStorageSagaStorage(StorageAccount, new ConsoleLoggerFactory(false), TableName, ContainerName);

                storage.Initialize();

                return storage;
            }
        }

        public void CleanUp()
        {

        }


        //public static void DropAndRecreateObjects()
        //{
        //    var cloudTableClient = StorageAccount.CreateCloudTableClient();

        //    var table = cloudTableClient.GetTableReference(TableName);
        //    table.DeleteIfExists();
        //    var cloudBlobClient = StorageAccount.CreateCloudBlobClient();
        //    var container = cloudBlobClient.GetContainerReference(ContainerName.ToLowerInvariant());
        //    container.DeleteIfExists();
        //}

    }
}

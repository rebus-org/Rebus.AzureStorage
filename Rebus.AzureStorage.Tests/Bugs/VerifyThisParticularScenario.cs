using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Rebus.AzureStorage.Tests.Extensions;
using Rebus.AzureStorage.Tests.Subscriptions;
using Rebus.Extensions;
using Rebus.Subscriptions;
using Rebus.Tests.Contracts;
using Sample.Common.DomainEvents;

namespace Rebus.AzureStorage.Tests.Bugs
{
    [TestFixture]
    [Description("Pretty sure there's no bug, it's just a reproduction with some actual queue and topic names. Here we go")]
    public class VerifyThisParticularScenario : FixtureBase
    {
        ISubscriptionStorage _storage;
        AzureStorageSubscriptionStorageFactory _factory;

        protected override void SetUp()
        {
            _factory = new AzureStorageSubscriptionStorageFactory();

            Using(_factory.AsDisposable(f => f.Cleanup()));

            _storage = _factory.Create();
        }

        [Test]
        public async Task ItWorksAsItShould()
        {
            var topic = typeof(InstitutionCreatedEventV3).GetSimpleAssemblyQualifiedName();

            Console.WriteLine($"Registering subscribers for topic '{topic}'");

            await _storage.RegisterSubscriber(topic, "readmodel-subscriber");
            await _storage.RegisterSubscriber(topic, "sample-subscriber");

            var subscribers = (await _storage.GetSubscriberAddresses(topic)).OrderBy(s => s).ToList();

            Console.WriteLine($"Retrieved subscribers for topic '{topic}': {string.Join(", ", subscribers)}");

            Assert.That(subscribers.Count, Is.EqualTo(2));
            Assert.That(subscribers, Is.EqualTo(new[]
            {
                "readmodel-subscriber",
                "sample-subscriber"
            }));
        }
    }
}

namespace Sample.Common.DomainEvents
{
    public class InstitutionCreatedEventV3 { }
}
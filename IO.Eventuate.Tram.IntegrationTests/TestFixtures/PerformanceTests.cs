﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using IO.Eventuate.Tram.Events.Common;
using IO.Eventuate.Tram.IntegrationTests.TestHelpers;
using IO.Eventuate.Tram.Messaging.Consumer.Kafka;
using NUnit.Framework;

namespace IO.Eventuate.Tram.IntegrationTests.TestFixtures
{
    [TestFixture]
    public class PerformanceTests : IntegrationTestsBase
    {
        [SetUp]
        public void Setup()
        {
            CleanupKafka();
            TestSetup("eventuate", false, EventuateKafkaConsumerConfigurationProperties.Empty());
            CleanupTest();
        }

        [TearDown]
        public void TearDown()
        {
            DisposeTestHost();
        }

        [Test]
        public void Send1000Message_Within1Minute()
        {
            // Arrange
            TestMessageType1 msg1 = new TestMessageType1("Msg1", 1, 1.2);
            TestEventConsumer consumer = GetTestConsumer();

            // Act
            for (int x = 0; x < 1000; x++)
            {
                GetTestPublisher().Publish(AggregateType, AggregateType, new List<IDomainEvent> { msg1 });
                GetDbContext().SaveChanges();
            }

            // Allow time for messages to process
            int count = 300;
            while (consumer.Type1MessageCount < 1000 && count > 0)
            {
                Thread.Sleep(1000);
                count--;
            }

            ShowTestResults();

            // Assert
            Assert.AreEqual(1000, GetDbContext().Messages.Count(), "Expect 1000 messages produced");
            Assert.AreEqual(1000, consumer.Type1MessageCount, "Received by consumer count must be 1000");
            Assert.AreEqual(0, GetDbContext().Messages.Count(msg => msg.Published == 0), "No unpublished messages");
            Assert.AreEqual(1000, GetDbContext().ReceivedMessages.Count(msg => msg.MessageId != null), "Expect 1000 messages received");
            Assert.Less(consumer.Type1Duration().TotalSeconds, 60.0, "Time must be less than 60 seconds");

            TestContext.WriteLine("Performance Test completed in {0} seconds", consumer.Type1Duration().TotalSeconds);
        }
    }
}
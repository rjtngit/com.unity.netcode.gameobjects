#if MULTIPLAYER_TOOLS
using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using Unity.Multiplayer.MetricTypes;
using Unity.Netcode.RuntimeTests.Metrics.Utlity;
using UnityEngine.TestTools;

namespace Unity.Netcode.RuntimeTests.Metrics
{
    public class ServerLogsMetricTests : SingleClientMetricTestBase
    {
        [UnityTest]
        public IEnumerator TrackServerLogSentMetric()
        {
            var waitForSentMetric = new WaitForMetricValues<ServerLogEvent>(ClientMetrics.Dispatcher, MetricNames.ServerLogSent);

            var message = Guid.NewGuid().ToString();
            NetworkLog.LogWarningServer(message);

            yield return waitForSentMetric.WaitForMetricsReceived();

            var sentMetrics = waitForSentMetric.AssertMetricValuesHaveBeenFound();
            Assert.AreEqual(1, sentMetrics.Count);

            var sentMetric = sentMetrics.First();
            Assert.AreEqual(Server.LocalClientId, sentMetric.Connection.Id);
            Assert.AreEqual((uint)NetworkLog.LogType.Warning, (uint)sentMetric.LogLevel);
            Assert.AreEqual(message.Length + 2, sentMetric.BytesCount);
        }

        [UnityTest]
        public IEnumerator TrackServerLogReceivedMetric()
        {
            var waitForReceivedMetric = new WaitForMetricValues<ServerLogEvent>(ServerMetrics.Dispatcher, MetricNames.ServerLogReceived);

            var message = Guid.NewGuid().ToString();
            NetworkLog.LogWarningServer(message);

            yield return waitForReceivedMetric.WaitForMetricsReceived();

            var receivedMetrics = waitForReceivedMetric.AssertMetricValuesHaveBeenFound();
            Assert.AreEqual(1, receivedMetrics.Count);

            var receivedMetric = receivedMetrics.First();
            Assert.AreEqual(Client.LocalClientId, receivedMetric.Connection.Id);
            Assert.AreEqual((uint)NetworkLog.LogType.Warning, (uint)receivedMetric.LogLevel);
            Assert.AreEqual(message.Length + 2, receivedMetric.BytesCount);
        }
    }
}
#endif
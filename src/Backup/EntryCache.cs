using System.Collections.Generic;
using AutoTest.Client.Logging;
using AutoTest.VM.Messages;
namespace AutoTest.VS.RiskClassifier
{
    public static class EntryCache
    { 
        private static readonly Dictionary<string, RiskEntry> Cache = new Dictionary<string, RiskEntry>();
        private static Communication communications = null;
        private static readonly object guard = new object();

        public static RiskEntry GetRiskEntryFor(string signature)
        {
            VerifyCommunications();
            RiskEntry entry;
            if(!Cache.TryGetValue(signature, out entry))
            {
                entry = new RiskEntry(signature, 0, 20, 0);
                Cache.Add(signature, entry);
                communications.UpdateEntry(entry.Signature);
            }
            if(entry.NeedsRefresh)
            {
                communications.UpdateEntry(entry.Signature);
            }
            return entry;
        }

        private static void VerifyCommunications()
        {
            if(communications == null)
            {
                lock (guard)
                {
                    if(communications == null)
                        communications = new Communication();
                }
            }
        }

        public static void InvalidateNoUpdate(string signature)
        {
            RiskEntry entry;
            if (Cache.TryGetValue(signature, out entry))
            {
                Logger.Write("invalidating " + signature);
                entry.InvalidateNoUpdate();
            }
        }

        public static void Update(RiskMetricGeneratedMessage message)
        {
            RiskEntry entry;
            if (Cache.TryGetValue(message.Signature, out entry))
            {
                Logger.Write("updating " + message.Signature + " count =" + message.NodeCount + " time = " + message.AverageTime + " called count = " + message.CalledCount);
                entry.SetNewData(message.NumberOfTests, message.Found, message.Descriptors, message.RiskMetric, message.NodeCount, message.CalledCount, message.AverageTime,
                    message.AverageTimeUnder, message.TestsScore, message.GraphScore, message.Complexity);
            }
        }

        public static void Invalidate() 
        {
            foreach(var item in Cache.Values)
            {
                item.Invalidate();
            }
        }

        public static void Update(string signature)
        {
            communications.UpdateEntry(signature);
        }
    }
}
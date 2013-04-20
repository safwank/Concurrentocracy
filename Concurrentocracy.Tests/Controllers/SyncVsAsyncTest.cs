using System;
using System.Diagnostics;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;
using RestSharp;

namespace Concurrentocracy.Tests.Controllers
{
    [TestClass]
    public class SyncVsAsyncTest
    {
        [TestMethod]
        public void Async_Should_Be_Faster_Than_Sync1()
        {
            var asyncTiming = HitAsyncAndGetTimeElapsed();
            var syncTiming = HitSyncAndGetTimeElapsed();

            asyncTiming.Should().BeLessThan(syncTiming);
        }

        [TestMethod]
        public void Async_Should_Be_Faster_Than_Sync2()
        {
            var syncTiming = HitSyncAndGetTimeElapsed();
            var asyncTiming = HitAsyncAndGetTimeElapsed();

            asyncTiming.Should().BeLessThan(syncTiming);
        }

        private static long HitSyncAndGetTimeElapsed()
        {
            return HitAndGetTimeElapsed(HitSync);
        }

        private static long HitAsyncAndGetTimeElapsed()
        {
            return HitAndGetTimeElapsed(HitAsync);
        }

        private static long HitAndGetTimeElapsed(Action hitAction)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Parallel.ForEach(Enumerable.Range(1, 50), i => hitAction());
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        [TestMethod]
        public void Run_Async_Only()
        {
            Parallel.ForEach(Enumerable.Range(1, 50), i => HitAsync());
        }

        [TestMethod]
        public void Run_Sync_Only()
        {
            Parallel.ForEach(Enumerable.Range(1, 50), i => HitSync());
        }

        private static void HitAsync()
        {
            HitIt("http://concurrentocracy.apphb.com/Async");
        }

        private static void HitSync()
        {
            HitIt("http://concurrentocracy.apphb.com/Sync");
        }

        private static void HitIt(string url)
        {
            var restClient = new RestClient(url);
            var request = new RestRequest(Method.GET);
            restClient.Get(request);
        }
    }
}
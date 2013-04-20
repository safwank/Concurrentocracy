using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Concurrentocracy.Tests.Controllers
{
    [TestClass]
    public class SyncVsAsyncTest
    {
        [TestMethod]
        public void Remote_Async_Should_Be_Faster()
        {
            var asyncTiming = HitAsyncAndGetTimeElapsed();
            var syncTiming = HitSyncAndGetTimeElapsed();

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
            QueueHits(hitAction);
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        [TestMethod]
        public void Run_Remote_Async_Only()
        {
            QueueHits(HitAsync);
        }

        [TestMethod]
        public void Run_Remote_Sync_Only()
        {
            QueueHits(HitSync);
        }

        private static void QueueHits(Action hitAction)
        {
            var tasks = new List<Task>();
            Enumerable.Range(1, 1000).ToList().ForEach(i =>
                                                           {
                                                               var task = Task.Factory.StartNew(hitAction);
                                                               tasks.Add(task);
                                                           });
            Task.WaitAll(tasks.ToArray());
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

        [TestMethod]
        public void Local_Async_Should_Be_Faster()
        {
            var iterations = Enumerable.Range(1, 10000).ToList();

            var tasks = new List<Task>();
            var stopWatch = Stopwatch.StartNew();
            iterations.ForEach(i =>
                                   {
                                       var task = Task.Delay(1);
                                       tasks.Add(task);
                                   });
            Task.WaitAll(tasks.ToArray());
            stopWatch.Stop();
            var asyncElapsed = stopWatch.ElapsedMilliseconds;
            Console.WriteLine(asyncElapsed);

            stopWatch = Stopwatch.StartNew();
            iterations.ForEach(i => Thread.Sleep(1));
            stopWatch.Stop();
            var syncElapsed = stopWatch.ElapsedMilliseconds;
            Console.WriteLine(syncElapsed);

            stopWatch = Stopwatch.StartNew();
            Parallel.ForEach(iterations, i => Thread.Sleep(1));
            stopWatch.Stop();
            var parallelElapsed = stopWatch.ElapsedMilliseconds;
            Console.WriteLine(parallelElapsed);

            asyncElapsed.Should().BeLessThan(syncElapsed);
            asyncElapsed.Should().BeLessThan(parallelElapsed);
        }
    }
}
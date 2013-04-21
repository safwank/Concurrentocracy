using System.Collections.Concurrent;
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
        private const string BaseUrl = "http://concurrentocracy.apphb.com/";

        [TestMethod]
        public void Remote_Async_Should_Be_Faster()
        {
            var syncTiming = HitSyncAndGetTimeElapsed();
            Console.WriteLine(syncTiming);

            var asyncTiming = HitAsyncAndGetTimeElapsed();
            Console.WriteLine(asyncTiming);

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

        private static long HitAndGetTimeElapsed(Func<string> hitFunc)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            QueueHits(hitFunc);
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        [TestMethod]
        public void Remote_Async_Should_Have_Higher_Throughput()
        {
            var asyncThroughput = HitAsyncAndGetCount();
            Console.WriteLine(asyncThroughput);

            var syncThroughput = HitSyncAndGetCount();
            Console.WriteLine(syncThroughput);

            asyncThroughput.Should().BeGreaterThan(syncThroughput);
        }

        private static int HitAsyncAndGetCount()
        {
            return HitAndGetNumberOfCompletedRequests(HitAsync);
        }

        private static int HitSyncAndGetCount()
        {
            return HitAndGetNumberOfCompletedRequests(HitSync);
        }

        private static int HitAndGetNumberOfCompletedRequests(Func<string> hitFunc)
        {
            var responses = new BlockingCollection<string>();
            var tasks = new List<Task>();
            Enumerable.Range(1, 1000)
                      .ToList()
                      .ForEach(i =>
                                   {
                                       var task = Task.Run(hitFunc)
                                                      .ContinueWith(response => responses.Add(response.Result));
                                       tasks.Add(task);
                                   });
            Task.WaitAll(tasks.ToArray(), TimeSpan.FromSeconds(30));
            return responses.Count;
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

        private static void QueueHits(Func<string> hitFunc)
        {
            var tasks = new List<Task>();
            Enumerable.Range(1, 100)
                      .ToList()
                      .ForEach(i =>
                                   {
                                       var task = Task.Run(hitFunc);
                                       tasks.Add(task);
                                   });
            Task.WaitAll(tasks.ToArray());
        }

        private static string HitAsync()
        {
            return HitIt(string.Concat(BaseUrl, "Async"));
        }

        private static string HitSync()
        {
            return HitIt(string.Concat(BaseUrl, "Sync"));
        }

        private static string HitIt(string url)
        {
            var restClient = new RestClient(url);
            var request = new RestRequest(Method.GET);
            var response = restClient.Get(request);
            return response.Content;
        }

        [TestMethod]
        public void Local_Async_Should_Be_Faster()
        {
            var iterations = Enumerable.Range(1, 100).ToList();

            var tasks = new List<Task>();
            var stopWatch = Stopwatch.StartNew();
            iterations.ForEach(i =>
                                   {
                                       var task = Task.Run(async () => await Task.Delay(1));
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
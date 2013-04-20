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
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            Parallel.ForEach(Enumerable.Range(1, 500),
                             i => HitIt("http://localhost:1268/Async"));
            stopwatch.Stop();
            var asyncTiming = stopwatch.ElapsedMilliseconds;

            stopwatch.Start();
            Parallel.ForEach(Enumerable.Range(1, 500),
                             i => HitIt("http://localhost:1268/Sync"));
            stopwatch.Stop();
            var syncTiming = stopwatch.ElapsedMilliseconds;

            asyncTiming.Should().BeLessThan(syncTiming);
        }

        [TestMethod]
        public void Async_Should_Be_Faster_Than_Sync2()
        {
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            Parallel.ForEach(Enumerable.Range(1, 500),
                             i => HitIt("http://localhost:1268/Sync"));
            stopwatch.Stop();
            var syncTiming = stopwatch.ElapsedMilliseconds;

            stopwatch.Start();
            Parallel.ForEach(Enumerable.Range(1, 500),
                             i => HitIt("http://localhost:1268/Async"));
            stopwatch.Stop();
            var asyncTiming = stopwatch.ElapsedMilliseconds;

            asyncTiming.Should().BeLessThan(syncTiming);
        }

        [TestMethod]
        public void Run_Async()
        {
            Parallel.ForEach(Enumerable.Range(1, 1000),
                             i => HitIt("http://localhost:1268/Async"));
        }

        [TestMethod]
        public void Run_Sync()
        {
            Parallel.ForEach(Enumerable.Range(1, 1000),
                             i => HitIt("http://localhost:1268/Sync"));
        }

        private static void HitIt(string url)
        {
            var restClient = new RestClient(url);
            var request = new RestRequest(Method.GET);
            restClient.Get(request);
        }
    }
}
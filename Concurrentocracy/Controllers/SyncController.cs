using System;
using System.Threading;
using System.Web.Http;

namespace Concurrentocracy.Controllers
{
    public class SyncController : ApiController
    {
        public DateTime Get()
        {
            var random = new Random();
            var sleepPeriod = random.Next(500, 1000);
            Thread.Sleep(sleepPeriod);
            Thread.Sleep(sleepPeriod);
            return DateTime.Now;
        }
    }
}

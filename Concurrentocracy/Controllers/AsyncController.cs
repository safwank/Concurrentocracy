using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace Concurrentocracy.Controllers
{
    public class AsyncController : ApiController
    {
        public async Task<DateTime> Get()
        {
            var random = new Random();
            var sleepPeriod = random.Next(500, 1000);
            await Task.Delay(sleepPeriod);
            await Task.Delay(sleepPeriod);
            return DateTime.Now;
        }
    }
}
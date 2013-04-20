using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Concurrentocracy.Controllers
{
    public class AsyncController : ApiController
    {
        public async Task<DateTime> Get()
        {
            return await Task.Factory.StartNew(() =>
                                                   {
                                                       var random = new Random();
                                                       var sleepPeriod = random.Next(500, 1000);
                                                       Thread.Sleep(sleepPeriod);
                                                       return DateTime.Now;
                                                   });
        }
    }
}
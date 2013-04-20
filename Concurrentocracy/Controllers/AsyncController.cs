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
                                                       Thread.Sleep(1000);
                                                       return DateTime.Now;
                                                   });
        }
    }
}
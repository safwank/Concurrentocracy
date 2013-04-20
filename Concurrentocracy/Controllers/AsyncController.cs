using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace Concurrentocracy.Controllers
{
    public class AsyncController : ApiController
    {
        public async Task<DateTime> Get()
        {
            await Task.Delay(10);
            await Task.Delay(10);
            return DateTime.Now;
        }
    }
}
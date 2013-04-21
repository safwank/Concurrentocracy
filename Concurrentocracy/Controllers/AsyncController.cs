using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace Concurrentocracy.Controllers
{
    public class AsyncController : ApiController
    {
        public async Task<string> Get()
        {
            await Task.Delay(50);
            await Task.Delay(50);
            return "async";
        }
    }
}
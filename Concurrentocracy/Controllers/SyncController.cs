using System;
using System.Threading;
using System.Web.Http;

namespace Concurrentocracy.Controllers
{
    public class SyncController : ApiController
    {
        public DateTime Get()
        {
            Thread.Sleep(1000);
            return DateTime.Now;
        }
    }
}

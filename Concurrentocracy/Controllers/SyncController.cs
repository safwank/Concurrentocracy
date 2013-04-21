using System;
using System.Threading;
using System.Web.Http;

namespace Concurrentocracy.Controllers
{
    public class SyncController : ApiController
    {
        public string Get()
        {
            Thread.Sleep(50);
            Thread.Sleep(50);
            return "sync";
        }
    }
}

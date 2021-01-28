using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BillTracker.Api.Controllers
{
    [Route("home")]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return "Hello world!";
        }
    }
}

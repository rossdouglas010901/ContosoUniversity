using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ContosoUniversity.Controllers
{
    public class ErrorController : Controller
    {
        [Route("/Error/{ErrorCode}")]
        public IActionResult Error(int ErrorCode)
        {
            return View(ErrorCode);
        }
    }
}
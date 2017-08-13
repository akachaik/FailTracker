using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FailTracker.Web.Models;

namespace FailTracker.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ISayHello _sayHello;

        public HomeController(ApplicationDbContext context, ISayHello sayHello)
        {
            _context = context;
            _sayHello = sayHello;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }

    public interface ISayHello
    {
        
    }

    public class SayHello : ISayHello
    {
        
    }
}
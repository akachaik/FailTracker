using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FailTracker.Web.Infrastructure;
using FailTracker.Web.Models;

namespace FailTracker.Web.Controllers
{
    public class HomeController : FailTrackerController
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public ActionResult Index()
        {
            return View();
        }

    }
}
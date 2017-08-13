using System.Web.Mvc;
using FailTracker.Web.Models;

namespace FailTracker.Web.Controllers
{
    public class IssueController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IssueController(ApplicationDbContext context)
        {
            _context = context;
        }
        public ActionResult IssueWidget()
        {

            return Content("Isssues go here.");

        }
    }
}
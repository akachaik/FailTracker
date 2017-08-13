using System;
using System.Linq;
using System.Web.Mvc;
using FailTracker.Web.Filters;
using FailTracker.Web.Infrastructure;
using FailTracker.Web.Models;
using Microsoft.AspNet.Identity;

namespace FailTracker.Web.Controllers
{
    public class IssueController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUser _currentUser;

        public IssueController(ApplicationDbContext context, ICurrentUser currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        [ChildActionOnly]
        public ActionResult IssueWidget()
        {
            var models = from i in _context.Issues
                select new IssueSummaryViewModel
                {
                    IssueId = i.IssueId,
                    Subject = i.Subject,
                    CreatedAt = i.CreatedAt
                };

            return PartialView(models.ToArray());

        }

        public ActionResult New()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Log("Created issue")]
        public ActionResult New(NewIssueForm form)
        {

            _context.Issues.Add(new Issue(_currentUser.User, form.Subject, form.Body));

            _context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        [Log("Viewed issue {id}")]
        public ActionResult View(int id)
        {
            var issue = _context.Issues.Find(id);
            if (issue == null)
            {
                throw new ApplicationException("Issue not found!");
            }

            return View(new IssueDetailsViewModel
            {
                IssueId = issue.IssueId,
                Subject = issue.Subject,
                CreatedAt = issue.CreatedAt,
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Log("Deleted issue {id}")]
        public ActionResult Delete(int id)
        {
            var issue = _context.Issues.Find(id);

            if (issue==null)
            {
                throw new ApplicationException("Issue not found!");
            }

            _context.Issues.Remove(issue);
            _context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }
    }

    public class IssueDetailsViewModel
    {
        public int IssueId { get; set; }
        public string Subject { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class NewIssueForm
    {
        public string Subject { get; set; }
        public string Body { get; set; }
    }

    public class IssueSummaryViewModel
    {
        public int IssueId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
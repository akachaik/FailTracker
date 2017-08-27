using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web.Mvc;
using AutoMapper.QueryableExtensions;
using FailTracker.Web.Filters;
using FailTracker.Web.Infrastructure;
using FailTracker.Web.Models;

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
        public ActionResult YourIssuesWidget()
        {

            var issues = _context.Issues.AsQueryable();

            if (_currentUser.User != null)
            {
                issues = issues.Where(i => i.AssignedTo.Id == _currentUser.User.Id);
            }
            else
            {
                issues = new List<Issue>().AsQueryable();
            }

            var models = issues.Project().To<IssueSummaryViewModel>();

            return PartialView(models.ToArray());

        }

        [ChildActionOnly]
        public ActionResult CreatedByYouWidget()
        {
            var issues = _context.Issues.AsQueryable();

            if (_currentUser.User != null)
            {
                issues = issues.Where(i => i.Creator.Id == _currentUser.User.Id).AsQueryable();
            }
            else
            {
                issues = new List<Issue>().AsQueryable();
            }

            var models = issues.Project().To<IssueSummaryViewModel>();

            return PartialView(models.ToArray());

        }

        [ChildActionOnly]
        public ActionResult AssignmentStatsWidget()
        {
            var stats = _context.Users.Select(u => new AssignmentStatsViewModel
            {
                UserName = u.UserName,
                Enhancements = u.Assignments.Count(i => i.IssueType == IssueType.Enhancement),
                Bugs = u.Assignments.Count(i => i.IssueType == IssueType.Bug),
                Support = u.Assignments.Count(i => i.IssueType == IssueType.Support),
                Other = u.Assignments.Count(i => i.IssueType == IssueType.Other),
            }).ToArray();

            return PartialView(stats);
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
            var issue = _context.Issues
                .Project().To<IssueDetailsViewModel>()
                .SingleOrDefault(i => i.IssueId == id);


            if (issue == null)
            {
                throw new ApplicationException("Issue not found!");
            }

            return View(issue);
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

        [Log("Started to edit issue {id}")]
        public ActionResult Edit(int id)
        {

            var form = _context.Issues
                .Project().To<EditIssueForm>()
                .SingleOrDefault(i => i.IssueId == id);
            if (form == null)
            {
                throw new ApplicationException("Issue not found");
            }

            form.AvailableUsers = GetAvailableUsers();
            form.AvailableIssueTypes = GetAvailableIssueTypes();

            return View(form);
        }

        private SelectListItem[] GetAvailableIssueTypes()
        {
            return Enum.GetValues(typeof(IssueType))
                .Cast<IssueType>()
                .Select(t => new SelectListItem {Text = t.ToString(), Value = t.ToString()})
                .ToArray();

        }

        private SelectListItem[] GetAvailableUsers()
        {
            return _context.Users.Select(u => new SelectListItem {Text = u.UserName, Value = u.Id}).ToArray();
        }
    }

    public class EditIssueForm
    {
        public int IssueId { get; set; }
        public string Subject { get; set; }
        public string AssignedToId { get; set; }
        public SelectListItem[] AvailableUsers { get; set; }
        public string CreatorUserName { get; set; }
        public IssueType IssueType { get; set; }
        public SelectListItem[] AvailableIssueTypes { get; set; }
        public string Body { get; set; }
    }

    public class AssignmentStatsViewModel
    {
        public string UserName { get; set; }
        public int Enhancements { get; set; }
        public int Bugs { get; set; }
        public int Support { get; set; }
        public int Other { get; set; }
    }

    public class IssueDetailsViewModel
    {
        public int IssueId { get; set; }
        public string Subject { get; set; }
        public DateTime CreatedAt { get; set; }
        public string AssignedToUserName { get; set; }
        public string CreatorUserName { get; set; }
    }

    public class NewIssueForm
    {
        public string Subject { get; set; }
        public string Body { get; set; }
    }

    public class IssueSummaryViewModel
    {
        public IssueSummaryViewModel()
        {
            
        }
        public int IssueId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public IssueType IssueType { get; set; }
        public string CreatorUserName { get; set; }
        public string AssignedToUserName { get; set; }
    }
}
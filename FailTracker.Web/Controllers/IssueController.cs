﻿using System;
using System.Linq;
using System.Web.Mvc;
using FailTracker.Web.Models;
using Microsoft.AspNet.Identity;

namespace FailTracker.Web.Controllers
{
    public class IssueController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IssueController(ApplicationDbContext context)
        {
            _context = context;
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
        public ActionResult New(NewIssueForm form)
        {
            var userId = User.Identity.GetUserId();
            var user = _context.Users.Find(userId);

            _context.Issues.Add(entity: new Issue(user, form.Subject, form.Body));

            _context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

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
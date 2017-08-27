using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Management;
using System.Web.Mvc;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FailTracker.Web.Filters;
using FailTracker.Web.Infrastructure;
using FailTracker.Web.Infrastructure.Alerts;
using FailTracker.Web.Infrastructure.Mapping;
using FailTracker.Web.Models;
using Microsoft.Web.Mvc;

namespace FailTracker.Web.Controllers
{
    public class IssueController : FailTrackerController
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
            return View(new NewIssueForm());
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Log("Created issue")]
        public ActionResult New(NewIssueForm form)
        {

            _context.Issues.Add(new Issue(_currentUser.User, form.Subject, form.Body));

            _context.SaveChanges();

            return RedirectToAction<HomeController>(c => c.Index())
                .WithSuccess("Issue created!");
        }

        [Log("Viewed issue {id}")]
        public ActionResult View(int id)
        {
            var issue = _context.Issues
                .Project().To<IssueDetailsViewModel>()
                .SingleOrDefault(i => i.IssueId == id);


            if (issue == null)
            {
                return RedirectToAction<HomeController>(c => c.Index())
                    .WithError("Unable to find the issue. Maybe it was deleted");
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
                return RedirectToAction<HomeController>(c => c.Index())
                    .WithError("Unable to find the issue. Maybe it was deleted");
            }

            _context.Issues.Remove(issue);
            _context.SaveChanges();

            return RedirectToAction<HomeController>(c => c.Index())
                .WithSuccess("Issue deleted!");
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

        [HttpPost, ValidateAntiForgeryToken]
        [Log("Edited issue")]
        public ActionResult Edit(EditIssueForm form)
        {
            var issue = _context.Issues
                .SingleOrDefault(i => i.IssueId == form.IssueId);

            if (issue == null)
            {
                return RedirectToAction<HomeController>(c => c.Index())
                    .WithError("Unable to find the issue. Maybe it was deleted");
            }

            var assignedToUser = _context.Users.Single(u => u.Id == form.AssignedToId);

            issue.Subject = form.Subject;
            issue.AssignedTo = assignedToUser;
            issue.Body = form.Body;
            issue.IssueType = form.IssueType;

            return this.RedirectToAction(c => c.View(form.IssueId))
                .WithSuccess("Changes saved!");
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

    public class EditIssueForm : IMapFrom<Issue>, IHaveUserSelectList, IHaveIssueTypeSelectList
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

    public class AssignmentStatsViewModel : IHaveCustomMappings
    {
        public string UserName { get; set; }
        public int Enhancements { get; set; }
        public int Bugs { get; set; }
        public int Support { get; set; }
        public int Other { get; set; }

        public void CreateMappings(IConfiguration configuration)
        {
            configuration.CreateMap<ApplicationUser, AssignmentStatsViewModel>()
                .ForMember(m => m.Enhancements, opt =>
                    opt.MapFrom(u => u.Assignments.Count(i => i.IssueType == IssueType.Enhancement)))
                .ForMember(m => m.Bugs, opt =>
                    opt.MapFrom(u => u.Assignments.Count(i => i.IssueType == IssueType.Bug)))
                .ForMember(m => m.Support, opt =>
                    opt.MapFrom(u => u.Assignments.Count(i => i.IssueType == IssueType.Support)))
                .ForMember(m => m.Other, opt =>
                    opt.MapFrom(u => u.Assignments.Count(i => i.IssueType == IssueType.Other)));
        }
    }

    public class IssueDetailsViewModel : IMapFrom<Issue>
    {
        public int IssueId { get; set; }
        public string Subject { get; set; }
        public DateTime CreatedAt { get; set; }
        public string AssignedToUserName { get; set; }
        public string CreatorUserName { get; set; }
    }

    public class NewIssueForm : IHaveUserSelectList, IHaveIssueTypeSelectList
    {
        [Required]
        public string Subject { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Body { get; set; }

        [Required]
        [Display(Name = "Issue Type")]
        public IssueType IssueType { get; set; }

        [Required]
        [Display(Name = "Assigned To")]
        public string AssignedToUserId { get; set; }

        public SelectListItem[] AvailableUsers { get; set; }
        public SelectListItem[] AvailableIssueTypes { get; set; }
    }

    public class IssueSummaryViewModel : IMapFrom<Issue>
    {
        public int IssueId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public IssueType IssueType { get; set; }
        public string CreatorUserName { get; set; }
        public string AssignedToUserName { get; set; }
    }
}
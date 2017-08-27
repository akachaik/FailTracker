using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FailTracker.Web.Controllers;
using FailTracker.Web.Infrastructure.Tasks;
using FailTracker.Web.Models;

namespace FailTracker.Web
{
    public class AutoMapperConfig : IRunAtInit
    {
        public void Execute()
        {
            Mapper.CreateMap<Issue, IssueSummaryViewModel>()
                .ForMember(m => m.Creator,
                    opt => opt.MapFrom(i => i.Creator.UserName))
                .ForMember(m => m.AssignedTo,
                    opt=> opt.MapFrom(i => i.AssignedTo.UserName))
                .ForMember(m => m.Type,
                    opt => opt.MapFrom(i => i.IssueType));

            Mapper.CreateMap<Issue, IssueDetailsViewModel>()
                .ForMember(m => m.AssignedTo,
                    opt => opt.MapFrom(i => i.AssignedTo.UserName))
                .ForMember(m => m.Creator,
                    opt => opt.MapFrom(i => i.Creator.UserName));
             
            Mapper.CreateMap<Issue, EditIssueForm>()
                .ForMember(m => m.AssignedToUserId,
                    opt => opt.MapFrom(i => i.AssignedTo.Id))
                .ForMember(m => m.Creator,
                    opt => opt.MapFrom(i => i.Creator.UserName));

            Mapper.CreateMap<ApplicationUser, AssignmentStatsViewModel>()
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
}

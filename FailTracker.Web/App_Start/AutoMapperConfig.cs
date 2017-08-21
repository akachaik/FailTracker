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
                .ForMember(m => m.IssueType,
                    opt => opt.MapFrom(i => i.IssueType));


        }
    }
}

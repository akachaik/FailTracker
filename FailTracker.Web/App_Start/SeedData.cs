using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FailTracker.Web.Infrastructure.Tasks;
using FailTracker.Web.Models;

namespace FailTracker.Web
{
    public class SeedData : IRunAtStartup
    {
        private readonly ApplicationDbContext _context;

        public SeedData(ApplicationDbContext context)
        {
            _context = context;
        }
        public void Execute()
        {
            if (!_context.Users.Any())
            {
                _context.Users.Add(new ApplicationUser
                {
                    UserName = "TestUser"
                });

                _context.SaveChanges();
            }
            if (!_context.Issues.Any())
            {
                var user = _context.Users.First();

                _context.Issues.Add(new Issue(user, "Test Issue 1", "Test Issue Body 1", _context.Users.First()));
                _context.Issues.Add(new Issue(user, "Test Issue 2", "Test Issue Body 2", _context.Users.First()));
                _context.Issues.Add(new Issue(user, "Test Issue 3", "Test Issue Body 3", _context.Users.First()));

                _context.SaveChanges();
            }

        }
    }
}

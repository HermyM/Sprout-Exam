using Microsoft.Extensions.DependencyInjection;
using Sprout.Exam.DataAccess;
using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sprout.Exam.Tests.MockDbContext.Data;

namespace Sprout.Exam.Tests.MockDbContext
{
    public static class MockDbContextFactory
    {
        public static EmployeeContext GetContext()
        {
            return GetContext(null);
        }
        public static EmployeeContext GetContext(Action<EmployeeContext> seedDataAction)
        {
            var serviceProvider = new ServiceCollection()
               .AddEntityFrameworkInMemoryDatabase()
               .BuildServiceProvider();

            var builder = new DbContextOptionsBuilder<EmployeeContext>();
            builder.UseInMemoryDatabase("TestEmployee")
                   .UseInternalServiceProvider(serviceProvider);

            var context = new EmployeeContext(builder.Options);

            if (seedDataAction == null)
            {
                SeedData(context);
            }
            else
            {
                seedDataAction(context);
            }

            return context;
        }

        private static void SeedData(EmployeeContext dbContext)
        {
            dbContext.Employee.AddRange(MockEmployee.GetEmployees());
            dbContext.SaveChanges();
        }
    }
}

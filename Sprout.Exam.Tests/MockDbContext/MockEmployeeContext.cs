using Microsoft.EntityFrameworkCore;
using Sprout.Exam.DataAccess.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sprout.Exam.Tests.MockDbContext
{
   public class MockEmployeeContext:DbContext
    {
        public MockEmployeeContext(DbContextOptions<MockEmployeeContext> options) : base(options)
        {

        }
        public virtual DbSet<Employee> Employee { get; set; }

        public void MarkUpdatedEntity<TEntity>(TEntity item) where TEntity : class
        {
            this.Set<TEntity>().Attach(item).State = EntityState.Modified;
        }
    }
}

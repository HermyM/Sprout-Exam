using Microsoft.EntityFrameworkCore;
using Sprout.Exam.DataAccess.Entity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sprout.Exam.DataAccess
{
    public class EmployeeContext:DbContext
    {
        public EmployeeContext(DbContextOptions<EmployeeContext> options): base(options)
        {

        }
        public virtual DbSet<Employee> Employee { get; set; }

        public void MarkUpdatedEntity<TEntity>(TEntity item) where TEntity : class
        {
            this.Set<TEntity>().Attach(item).State = EntityState.Modified;
        }
    }
}

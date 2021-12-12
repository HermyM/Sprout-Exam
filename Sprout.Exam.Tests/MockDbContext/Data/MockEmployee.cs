using Sprout.Exam.DataAccess.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sprout.Exam.Tests.MockDbContext.Data
{
    public class MockEmployee
    {
        public static List<Employee> GetEmployees()
        {
            var employees = new List<Employee>
            {
                new Employee{
                    Id = 1,
                    Birthdate = Convert.ToDateTime("02/20/1995"),
                    FullName = "Juan Dela Cruz",
                    isDeleted = false,
                    Tin = "123456789",
                    TypeId = 1
                },
                new Employee{
                    Id = 2,
                    Birthdate = Convert.ToDateTime("08/30/1995"),
                    FullName = "Juana Dela Cruz",
                    isDeleted = false,
                    Tin = "987654321",
                    TypeId = 2
                }
            };
            return employees;
        }
    }
}

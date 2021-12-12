
using Microsoft.EntityFrameworkCore;
using Sprout.Exam.Business.DataTransferObjects;
using Sprout.Exam.DataAccess.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sprout.Exam.DataAccess
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly EmployeeContext _context;
        public EmployeeRepository(EmployeeContext context)
        {
            _context = context;
        }

        public async Task<int> CreateEmployee(CreateEmployeeDto employee)
        {
            _context.Employee.Add(new Employee
            {
                Birthdate = employee.Birthdate,
                FullName = employee.FullName,
                Tin = employee.Tin,
                TypeId = employee.TypeId
            });
            await _context.SaveChangesAsync();

            return await _context.Employee.MaxAsync(c => c.Id);
        }

        public async Task<Employee> DeleteEmployee(int id)
        {
            var employee = await _context.Employee.FirstOrDefaultAsync(e => e.Id == id);
            if (employee == null)
                return null;
            employee.isDeleted = true;
            _context.MarkUpdatedEntity<Employee>(employee);
            await _context.SaveChangesAsync();
            return employee;
        }

        public async Task<EditEmployeeDto> EditEmployee(EditEmployeeDto employee)
        {
            var employeeToBeUpdated = await _context.Employee.FirstOrDefaultAsync(e => e.Id == employee.Id);
            if (employeeToBeUpdated == null)
                return null;

            employeeToBeUpdated.Birthdate = employee.Birthdate;
            employeeToBeUpdated.FullName = employee.FullName;
            employeeToBeUpdated.Tin = employee.Tin;
            employeeToBeUpdated.TypeId = employee.TypeId;
            _context.MarkUpdatedEntity<Employee>(employeeToBeUpdated);
            await _context.SaveChangesAsync();
            return employee;
        }

        public async Task<List<EmployeeDto>> GetAll()
        {
            return await _context.Employee
                                 .Where(e => !e.isDeleted)
                                 .Select(e => new EmployeeDto
                                 {
                                     Birthdate = e.Birthdate.ToString("yyyy-MM-dd"),
                                     FullName = e.FullName,
                                     Id = e.Id,
                                     Tin = e.Tin,
                                     TypeId = e.TypeId,
                                 })
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        public async Task<EmployeeDto> GetEmployeeById(int Id)
        {
            return await _context.Employee
                                 .Select(e => new EmployeeDto
                                 {
                                     Birthdate = e.Birthdate.ToString("yyyy-MM-dd"),
                                     FullName = e.FullName,
                                     Id = e.Id,
                                     Tin = e.Tin,
                                     TypeId = e.TypeId,
                                 })
                               .AsNoTracking()
                               .FirstOrDefaultAsync(e => e.Id == Id);
        }

        public async Task<bool> isEmployeeExists(string tin, int? id = 0)
        {
            //For Create
            if (id == 0)
            {
                return await _context.Employee.AnyAsync(e => e.Tin == tin
                                                      && !e.isDeleted);
            }
            //For Update
            else
            {
                return await _context.Employee.AnyAsync(e => (e.Tin == tin && e.Id != id.Value)
                                                          && !e.isDeleted);
            }
        }

    }
}

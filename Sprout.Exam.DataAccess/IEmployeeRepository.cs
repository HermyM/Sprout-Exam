

using Sprout.Exam.Business.DataTransferObjects;
using Sprout.Exam.DataAccess.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sprout.Exam.DataAccess
{
    public interface IEmployeeRepository
    {
        Task<List<EmployeeDto>> GetAll();

        Task<EmployeeDto> GetEmployeeById(int Id);

        Task<EditEmployeeDto> EditEmployee(EditEmployeeDto employee);

        Task<int> CreateEmployee(CreateEmployeeDto employee);

        Task<Employee> DeleteEmployee(int id);

        Task<bool> isEmployeeExists(string tin,int? id);

    }
}

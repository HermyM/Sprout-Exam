using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Sprout.Exam.Business.DataTransferObjects;
using Sprout.Exam.Common.Enums;
using Sprout.Exam.DataAccess;

namespace Sprout.Exam.WebApp.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private IEmployeeRepository _service;
        private EmployeeContext _context;
        public EmployeesController(IEmployeeRepository service, EmployeeContext context)
        {
            _context = context;
            _service = new EmployeeRepository(_context);
        }
        /// <summary>
        /// Refactor this method to go through proper layers and fetch from the DB.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var employees = await _service.GetAll();
            return Ok(employees);
        }

        /// <summary>
        /// Refactor this method to go through proper layers and fetch from the DB.
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var employee = await _service.GetEmployeeById(id);
            if (employee == null)
                return NotFound("Employee don't exists!!");
            return Ok(employee);
        }

        /// <summary>
        /// Refactor this method to go through proper layers and update changes to the DB.
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(EditEmployeeDto input)
        {
            if (string.IsNullOrEmpty(input.FullName) ||
               string.IsNullOrEmpty(input.Birthdate.ToShortDateString()) ||
               string.IsNullOrEmpty(input.Tin) || input.TypeId == 0 || input.Id == 0)
                return BadRequest("Required Fields Validation Fails!!");

            if (await _service.isEmployeeExists(input.Tin, input.Id))
                return NotFound("Update Fails!!");

            var employee = await _service.EditEmployee(input);
            if (employee == null)
                return NotFound("Update Fails!!");

            return Ok(employee);

        }

        /// <summary>
        /// Refactor this method to go through proper layers and insert employees to the DB.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post(CreateEmployeeDto input)
        {
            if (string.IsNullOrEmpty(input.FullName) ||
               string.IsNullOrEmpty(input.Birthdate.ToShortDateString()) ||
               string.IsNullOrEmpty(input.Tin) ||
               input.TypeId == 0)
                return BadRequest("Required Fields Validation Fails!!");
            

            if (await _service.isEmployeeExists(input.Tin,0))
                return Ok("Employee exists!!");

            var id = await _service.CreateEmployee(input);

            return Created($"/api/employees/{id}", id);
        }

        /// <summary>
        /// Refactor this method to go through proper layers and perform soft deletion of an employee to the DB.
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _service.DeleteEmployee(id);
            if (employee == null)
                return NotFound("Employee don't exists!!");
            return Ok(id);
        }



        /// <summary>
        /// Refactor this method to go through proper layers and use Factory pattern
        /// </summary>
        /// <param name="id"></param>
        /// <param name="absentDays"></param>
        /// <param name="workedDays"></param>
        /// <returns></returns>
        [HttpPost("{id}/{absentDays}/{workedDays}/calculate")]
        public async Task<IActionResult> Calculate(int id, decimal absentDays, decimal workedDays)
        {

            var employee = await _service.GetEmployeeById(id);

            if (employee == null) return NotFound();

            var type = (EmployeeType)employee.TypeId;
            var salary = 0.00m;
            switch (type)
            {
                case EmployeeType.Regular:
                    var monthlyRate = 20000.00m;
                    var workDays = 22.00m;
                    var tax = .12m;

                    var daily = monthlyRate / workDays;
                    var absentDeduction = daily * absentDays;
                    var taxDeduction = monthlyRate * tax;
                    salary = Math.Round(monthlyRate - (absentDeduction + taxDeduction), 2);
                    break;
                case EmployeeType.Contractual:
                    var dailyRate = 500.00m;
                    salary = Math.Round(workedDays * dailyRate, 2);
                    break;
                default:
                    break;
            }

            return Ok(salary);

        }


    }
}

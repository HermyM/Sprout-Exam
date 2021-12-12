using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sprout.Exam.Business.DataTransferObjects;
using Sprout.Exam.DataAccess;
using Sprout.Exam.DataAccess.Entity;
using Sprout.Exam.Tests.MockDbContext;
using Sprout.Exam.WebApp.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sprout.Exam.Tests
{
    [TestClass]
    public class EmployeeTests
    {
        private  EmployeeContext _mockContext;
        private IEmployeeRepository _mockService;

        [TestMethod]
        public async Task GetAllEmployeeTest()
        {
            ConfigureContextAndInterface();
            var controller = new EmployeesController(_mockService, _mockContext);
            var result = await controller.Get() as ObjectResult;
            var employees = (List<EmployeeDto>)result.Value;

            Assert.AreEqual(HttpStatusCode.OK,(HttpStatusCode)result.StatusCode);
            Assert.AreEqual(2, employees.Count);
            Assert.IsTrue(employees.Any(e => e.FullName == "Juan Dela Cruz"));
            Assert.IsTrue(employees.Any(e => e.FullName == "Juana Dela Cruz"));
        }

        [TestMethod]
        public async Task GetEmployeeByIdTest()
        {
            ConfigureContextAndInterface();
            var controller = new EmployeesController(_mockService, _mockContext);
            var result = await controller.GetById(1) as ObjectResult;
            var employee =  (EmployeeDto)result.Value;

            Assert.AreEqual(HttpStatusCode.OK, (HttpStatusCode)result.StatusCode);
            Assert.IsTrue(employee.FullName == "Juan Dela Cruz");
        }

        [TestMethod]
        public async Task CreateEmployeeTest()
        {
            ConfigureContextAndInterface();
            var controller = new EmployeesController(_mockService, _mockContext);
            var newEmployee = new CreateEmployeeDto
            {
                FullName = "Joaquin Dela Cruz",
                Birthdate = Convert.ToDateTime("09/16/1990"),
                Tin = "564789321",
                TypeId = 1

            };
            //Check Db Before Insert
            var dbEmployeeBeforeInsert = _mockContext.Employee.ToList();
            Assert.AreEqual(2, dbEmployeeBeforeInsert.Count);

            var result = await controller.Post(newEmployee) as ObjectResult;
            var employeeID = result.Value;

            Assert.AreEqual(HttpStatusCode.Created, (HttpStatusCode)result.StatusCode);
            Assert.AreEqual(3, employeeID);

            //Check Db After Insert
            var dbEmployeeAfterInsert = _mockContext.Employee.ToList();
            Assert.AreEqual(3, dbEmployeeAfterInsert.Count);
            Assert.IsTrue(dbEmployeeAfterInsert.Any(e => e.Id == (int)employeeID
                                                      &&  e.FullName == "Joaquin Dela Cruz"));
        }

        [TestMethod]
        public async Task DeleteEmployeeTest()
        {
            ConfigureContextAndInterface();
            var controller = new EmployeesController(_mockService, _mockContext);
            //Check Db Before Delete
            var dbEmployeeBeforeDelete = _mockContext.Employee.Where(e => !e.isDeleted).ToList();
            Assert.AreEqual(2, dbEmployeeBeforeDelete.Count);

            var result = await controller.Delete(1) as ObjectResult;
            var employeeID = result.Value;

            Assert.AreEqual(HttpStatusCode.OK, (HttpStatusCode)result.StatusCode);
            Assert.AreEqual(1, employeeID);

            //Check Db After Delete
            //Remaining Employee
            var dbEmployeeAfterDelete = _mockContext.Employee.Where(e => !e.isDeleted).ToList();
            Assert.AreEqual(1, dbEmployeeAfterDelete.Count);

            //Deleted Employee
            var dbDeletedEmployee = _mockContext.Employee.FirstOrDefault(e => e.Id == (int)employeeID);
            Assert.IsTrue(dbDeletedEmployee.isDeleted);
        }

        [TestMethod]
        public async Task UpdateEmployeeTest()
        {
            ConfigureContextAndInterface();
            var controller = new EmployeesController(_mockService, _mockContext);

            var editedEmployee = new EditEmployeeDto
            {
                Id = 1,
                Birthdate = Convert.ToDateTime("02/22/1992"),
                FullName = "Juan Dela Cruz Edited",
                Tin = "0000000",
                TypeId = 2

            };
            //Check Db Before Edit
            //Original Value
            var dbEmployeeBeforeUPdate= _mockContext.Employee.FirstOrDefault(e => e.Id == 1);
            Assert.IsTrue(dbEmployeeBeforeUPdate.FullName == "Juan Dela Cruz" &&
                          dbEmployeeBeforeUPdate.Birthdate.ToShortDateString() == "2/20/1995" &&
                          dbEmployeeBeforeUPdate.Tin == "123456789" &&
                          dbEmployeeBeforeUPdate.TypeId ==  1);

            var result = await controller.Put(editedEmployee) as ObjectResult;
            var employee = (EditEmployeeDto)result.Value;

            Assert.AreEqual(HttpStatusCode.OK, (HttpStatusCode)result.StatusCode);

            //Check Db After Update
            var dbUpdatedEmployee = _mockContext.Employee.FirstOrDefault(e => e.Id == employee.Id);
            Assert.IsTrue(dbEmployeeBeforeUPdate.FullName == "Juan Dela Cruz Edited" &&
                         dbEmployeeBeforeUPdate.Birthdate.ToShortDateString() == "2/22/1992" &&
                         dbEmployeeBeforeUPdate.Tin == "0000000" &&
                         dbEmployeeBeforeUPdate.TypeId == 2);
        }

        [TestMethod]
        public async Task CalculateSalaryTest()
        {
            ConfigureContextAndInterface();
            var controller = new EmployeesController(_mockService, _mockContext);

            //Regular
            var result = await controller.Calculate(1,1,0) as ObjectResult;
            Assert.AreEqual(16690.91m, result.Value);

            //Contractual
            result = await controller.Calculate(2, 0, 15.5m) as ObjectResult;
            Assert.AreEqual(7750.00m, result.Value);
        }

        [TestMethod]
        public async Task GetEmployeeByIdDontExistsTest()
        {
            ConfigureContextAndInterface();
            var controller = new EmployeesController(_mockService, _mockContext);
            var result = await controller.GetById(3) as ObjectResult;

            Assert.AreEqual(HttpStatusCode.NotFound, (HttpStatusCode)result.StatusCode);
            Assert.AreEqual("Employee don't exists!!", result.Value);
        }

        [TestMethod]
        public async Task CreateEmployeeWithNullValueTest()
        {
            ConfigureContextAndInterface();
            var controller = new EmployeesController(_mockService, _mockContext);
            var newEmployee = new CreateEmployeeDto
            {
                Birthdate = Convert.ToDateTime("09/16/1990"),
                Tin = "",
                TypeId = 1

            };

            var result = await controller.Post(newEmployee) as ObjectResult;
            Assert.AreEqual(HttpStatusCode.BadRequest, (HttpStatusCode)result.StatusCode);
            Assert.AreEqual("Required Fields Validation Fails!!", result.Value.ToString());
        }

        [TestMethod]
        public async Task UpdateEmployeeWithNullValueTest()
        {
            ConfigureContextAndInterface();
            var controller = new EmployeesController(_mockService, _mockContext);
            var editedEmployee = new EditEmployeeDto
            {
                Birthdate = Convert.ToDateTime("02/22/1992"),
                FullName = "Juan Dela Cruz Edited",
            };

            var result = await controller.Put(editedEmployee) as ObjectResult;
            Assert.AreEqual(HttpStatusCode.BadRequest, (HttpStatusCode)result.StatusCode);
            Assert.AreEqual("Required Fields Validation Fails!!", result.Value.ToString());
        }

        [TestMethod]
        public async Task DeleteEmployeeDontExistsTest()
        {
            ConfigureContextAndInterface();
            var controller = new EmployeesController(_mockService, _mockContext);
            var result = await controller.Delete(3) as ObjectResult;

            Assert.AreEqual(HttpStatusCode.NotFound, (HttpStatusCode)result.StatusCode);
            Assert.AreEqual("Employee don't exists!!", result.Value);
        }

        [TestMethod]
        public async Task CreateEmployeeWithExistingTinTest()
        {
            ConfigureContextAndInterface();
            var controller = new EmployeesController(_mockService, _mockContext);
            var newEmployee = new CreateEmployeeDto
            {
                Birthdate = Convert.ToDateTime("02/20/1995"),
                FullName = "New Employee",
                Tin = "123456789",
                TypeId = 1
            };

            var result = await controller.Post(newEmployee) as ObjectResult;
            Assert.AreEqual(HttpStatusCode.OK, (HttpStatusCode)result.StatusCode);
            Assert.AreEqual("Employee exists!!", result.Value.ToString());
        }

        [TestMethod]
        public async Task UpdateEmployeeTinWithExistingTinTest()
        {
            ConfigureContextAndInterface();
            var controller = new EmployeesController(_mockService, _mockContext);
            var editedEmployee = new EditEmployeeDto
            {
                Id = 2,
                Birthdate = Convert.ToDateTime("08/30/1995"),
                FullName = "Juana Dela Cruz",
                Tin = "123456789",
                TypeId = 2
            };

            var result = await controller.Put(editedEmployee) as ObjectResult;
            Assert.AreEqual(HttpStatusCode.NotFound, (HttpStatusCode)result.StatusCode);
            Assert.AreEqual("Update Fails!!", result.Value.ToString());
        }

        private void ConfigureContextAndInterface()
        {
            _mockContext = MockDbContextFactory.GetContext();
            _mockService = new EmployeeRepository(_mockContext);
        }
    }








    
}

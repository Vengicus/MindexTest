using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using challenge.Services;
using challenge.Models;
using System.Globalization;

namespace challenge.Controllers
{
    [Route("api/employee")]
    public class EmployeeController : Controller
    {
        private readonly ILogger _logger;
        private readonly IEmployeeService _employeeService;

        public EmployeeController(ILogger<EmployeeController> logger, IEmployeeService employeeService)
        {
            _logger = logger;
            _employeeService = employeeService;
        }

        /// <summary>
        /// Add new employees to the DB
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult CreateEmployee([FromBody] Employee employee)
        {
            _logger.LogDebug($"Received employee create request for '{employee.FirstName} {employee.LastName}'");

            _employeeService.Create(employee);

            return CreatedAtRoute("getEmployeeById", new { id = employee.EmployeeId }, employee);
        }

        /// <summary>
        /// Get an employee by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "getEmployeeById")]
        public IActionResult GetEmployeeById(string id)
        {
            _logger.LogDebug($"Received employee get request for '{id}'");

            var employee = _employeeService.GetById(id);

            if (employee == null)
                return NotFound();

            return Ok(employee);
        }

        /// <summary>
        /// Replaces (deletes and adds) an existing employee with a new one
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newEmployee"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public IActionResult ReplaceEmployee(string id, [FromBody]Employee newEmployee)
        {
            _logger.LogDebug($"Recieved employee update request for '{id}'");

            var existingEmployee = _employeeService.GetById(id);
            if (existingEmployee == null)
                return NotFound();

            _employeeService.Replace(existingEmployee, newEmployee);

            return Ok(newEmployee);
        }

        /// <summary>
        /// Pulls back the full employee reporting structure alongside how many people that one person has reporting to them (NumberOfReports)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("numberReports/{id}")]
        public IActionResult GetReportingStructure(string id)
        {
            var employee = _employeeService.GetById(id);
            if (employee == null)
                return NotFound("No employee found for " + id);

            int numberOfReports = FlattenHierarchy(employee).Count() - 1; // Our hierarchy flattening includes our current employee, so let's just exclude them from the final count
            return Ok(new ReportingStructure()
            {
                Employee = employee,
                NumberOfReports = numberOfReports
            });
        }

        /// <summary>
        /// Gets the salary for an employee
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("compensation/{id}", Name = "getCompensationById")]
        public IActionResult GetEmployeeCompensation(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                var compensation = _employeeService.GetCompensationById(id);
                if (compensation == null)
                    return NotFound("No compensation information found for " + id);

                return Ok(compensation);
            }
            return BadRequest();

        }

        /// <summary>
        /// Sets the salary for an employee
        /// </summary>
        /// <param name="id"></param>
        /// <param name="salary"></param>
        /// <returns></returns>
        [HttpPut("compensation/{id}")]
        public IActionResult AddEmployeeCompensation(string id, [FromBody]string salary)
        {
            // First check if we have an ID and a valid dollar amount before we attempt to add
            if(!string.IsNullOrWhiteSpace(id) && 
               decimal.TryParse(salary, NumberStyles.Currency, CultureInfo.CreateSpecificCulture("en-US"), out decimal validSalary))
            {
                Compensation comp = _employeeService.UpsertCompensation(id, validSalary);
                if (comp != null)
                    return CreatedAtRoute("getCompensationById", new { id = comp.Employee.EmployeeId }, comp);
            }

            return BadRequest("Invalid ID or Salary was provided.");
        }

        /// <summary>
        /// Recursive method that will loop through the entire nested employee node list and flatten it
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        private IEnumerable<Employee> FlattenHierarchy(Employee employee)
        {
            yield return employee;
            if (employee.DirectReports != null)
            {
                foreach (var empl in employee.DirectReports.SelectMany(e => FlattenHierarchy(e)))
                    yield return empl;  //Yield return will maintain place in method so that we can go through the whole heirarchy and continue flattening
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using challenge.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using challenge.Data;

namespace challenge.Repositories
{
    public class EmployeeRespository : IEmployeeRepository
    {
        private readonly EmployeeContext _employeeContext;
        private readonly ILogger<IEmployeeRepository> _logger;

        public EmployeeRespository(ILogger<IEmployeeRepository> logger, EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
            _logger = logger;
        }

        public Employee Add(Employee employee)
        {
            employee.EmployeeId = Guid.NewGuid().ToString();
            _employeeContext.Employees.Add(employee);
            return employee;
        }

        public Employee GetById(string id)
        {
            // Load the data in first, without doing this it causes an issue where directReports will always return null (unless examined in the debugger to force a load)
            _employeeContext.Employees.Load();
            return _employeeContext.Employees.SingleOrDefault(e => e.EmployeeId == id);
        }

        public Task SaveAsync()
        {
            return _employeeContext.SaveChangesAsync();
        }

        public Employee Remove(Employee employee)
        {
            return _employeeContext.Remove(employee).Entity;
        }

        public Compensation AddCompensation(Compensation compensation)
        {
            _employeeContext.Compensation.Add(compensation);
            return compensation;
        }

        public Compensation UpdateCompensation(Compensation compensation)
        {
            _employeeContext.Compensation.Update(compensation);
            return compensation;
        }

        public Compensation GetCompensationById(string id)
        {
            // If we don't load in our entity data it will come back null in the response
            _employeeContext.Compensation.Load();
            _employeeContext.Employees.Load();
            Compensation compensation = _employeeContext.Compensation.SingleOrDefault(e => e.Employee.EmployeeId == id);
            return compensation;
        }
    }
}

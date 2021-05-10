using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using challenge.Models;
using Microsoft.Extensions.Logging;
using challenge.Repositories;

namespace challenge.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(ILogger<EmployeeService> logger, IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        public Employee Create(Employee employee)
        {
            if(employee != null)
            {
                _employeeRepository.Add(employee);
                _employeeRepository.SaveAsync().Wait();
            }

            return employee;
        }

        public Employee GetById(string id)
        {
            if(!String.IsNullOrEmpty(id))
            {
                return _employeeRepository.GetById(id);
            }

            return null;
        }

        public Employee Replace(Employee originalEmployee, Employee newEmployee)
        {
            if(originalEmployee != null)
            {
                _employeeRepository.Remove(originalEmployee);
                if (newEmployee != null)
                {
                    // ensure the original has been removed, otherwise EF will complain another entity w/ same id already exists
                    _employeeRepository.SaveAsync().Wait();

                    _employeeRepository.Add(newEmployee);
                    // overwrite the new id with previous employee id
                    newEmployee.EmployeeId = originalEmployee.EmployeeId;
                }
                _employeeRepository.SaveAsync().Wait();
            }

            return newEmployee;
        }

        public Compensation GetCompensationById(string id)
        {
            return _employeeRepository.GetCompensationById(id);
        }

        /// <summary>
        /// Handles updating and inserting employee salaries
        /// </summary>
        /// <param name="id"></param>
        /// <param name="salary"></param>
        /// <returns></returns>
        public Compensation UpsertCompensation(string id, decimal salary)
        {
            Employee empl = GetById(id);
            if (empl != null)
            {
                Compensation comp = _employeeRepository.GetCompensationById(id);
                if (comp == null)
                {
                    comp = _employeeRepository.AddCompensation(new Compensation()
                    {
                        Employee = empl,
                        Salary = salary,
                        EffectiveDate = DateTime.Now,
                    });
                }
                else
                {
                    comp.Salary = salary;
                    _employeeRepository.UpdateCompensation(comp);
                }
                _employeeRepository.SaveAsync().Wait();
                return comp;
            }
            return null;
        }
    }
}

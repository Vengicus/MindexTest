using challenge.Models;
using System;
using System.Threading.Tasks;

namespace challenge.Repositories
{
    public interface IEmployeeRepository
    {
        Employee GetById(string id);
        Employee Add(Employee employee);
        Employee Remove(Employee employee);
        Task SaveAsync();
        Compensation AddCompensation(Compensation compensation);
        Compensation GetCompensationById(string id);
        Compensation UpdateCompensation(Compensation compensation);
    }
}
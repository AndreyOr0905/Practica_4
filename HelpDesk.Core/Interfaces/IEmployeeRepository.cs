using System;
using System.Collections.Generic;
using System.Text;
using HelpDesk.Core.Models;

namespace HelpDesk.Core.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<Employee?> GetByIdAsync(int id);
        Task<List<Employee>> GetAllActiveAsync();
        Task<int> CreateAsync(Employee employee);
        Task SoftDeleteAsync(int id);
    }
}

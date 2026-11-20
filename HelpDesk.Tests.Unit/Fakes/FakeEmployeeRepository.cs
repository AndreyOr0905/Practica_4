using System;
using System.Collections.Generic;
using System.Text;
using HelpDesk.Core.Interfaces;
using HelpDesk.Core.Models;

namespace HelpDesk.Tests.Unit.Fakes
{
    public class FakeEmployeeRepository : IEmployeeRepository
    {
        public List<Employee> Items { get; } = new();

        public Task<Employee?> GetByIdAsync(int id) =>
            Task.FromResult(Items.FirstOrDefault(e => e.Id == id));

        public Task<List<Employee>> GetAllActiveAsync() =>
            Task.FromResult(Items.Where(e => !e.IsDeleted).ToList());

        public Task<int> CreateAsync(Employee employee)
        {
            employee.Id = Items.Count == 0 ? 1 : Items.Max(e => e.Id) + 1;
            Items.Add(employee);
            return Task.FromResult(employee.Id);
        }

        public Task SoftDeleteAsync(int id)
        {
            var emp = Items.FirstOrDefault(e => e.Id == id);
            if (emp != null) emp.IsDeleted = true;
            return Task.CompletedTask;
        }
    }
}

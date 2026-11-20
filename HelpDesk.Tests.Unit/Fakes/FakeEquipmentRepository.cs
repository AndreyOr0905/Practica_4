using System;
using System.Collections.Generic;
using System.Text;
using HelpDesk.Core.Interfaces;
using HelpDesk.Core.Models;

namespace HelpDesk.Tests.Unit.Fakes
{
    public class FakeEquipmentRepository : IEquipmentRepository
    {
        public List<Equipment> Items { get; } = new();

        public Task<Equipment?> GetByIdAsync(int id) =>
            Task.FromResult(Items.FirstOrDefault(e => e.Id == id));

        public Task<List<Equipment>> GetAllActiveAsync() =>
            Task.FromResult(Items.Where(e => !e.IsDeleted).ToList());

        public Task<int> CreateAsync(Equipment equipment)
        {
            equipment.Id = Items.Count == 0 ? 1 : Items.Max(e => e.Id) + 1;
            Items.Add(equipment);
            return Task.FromResult(equipment.Id);
        }

        public Task SoftDeleteAsync(int id)
        {
            var eq = Items.FirstOrDefault(e => e.Id == id);
            if (eq != null) eq.IsDeleted = true;
            return Task.CompletedTask;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using HelpDesk.Core.Models;

namespace HelpDesk.Core.Interfaces
{
    public interface IEquipmentRepository
    {
        Task<Equipment?> GetByIdAsync(int id);
        Task<List<Equipment>> GetAllActiveAsync();
        Task<int> CreateAsync(Equipment equipment);
        Task SoftDeleteAsync(int id);
    }
}

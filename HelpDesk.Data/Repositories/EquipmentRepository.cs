using HelpDesk.Core.Interfaces;
using HelpDesk.Data.EfModels;
using Microsoft.EntityFrameworkCore;
using CoreEquipment = HelpDesk.Core.Models.Equipment;
using EfEquipment = HelpDesk.Data.EfModels.Equipment;

namespace HelpDesk.Data.Repositories;

public class EquipmentRepository : IEquipmentRepository
{
    private readonly DbContextOptions<HelpDeskDbContext> _options;

    public EquipmentRepository(DbContextOptions<HelpDeskDbContext> options)
    {
        _options = options;
    }

    public async Task<CoreEquipment?> GetByIdAsync(int id)
    {
        await using var db = new HelpDeskDbContext(_options);
        var eq = await db.Equipment.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (eq == null) return null;

        return new CoreEquipment
        {
            Id = eq.Id,
            InventoryNumber = eq.InventoryNumber,
            Name = eq.Name,
            Department = eq.Department,
            IsDeleted = eq.IsDeleted
        };
    }

    public async Task<List<CoreEquipment>> GetAllActiveAsync()
    {
        await using var db = new HelpDeskDbContext(_options);
        return await db.Equipment
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Name)
            .Select(eq => new CoreEquipment
            {
                Id = eq.Id,
                InventoryNumber = eq.InventoryNumber,
                Name = eq.Name,
                Department = eq.Department,
                IsDeleted = eq.IsDeleted
            })
            .ToListAsync();
    }

    public async Task<int> CreateAsync(CoreEquipment equipment)
    {
        await using var db = new HelpDeskDbContext(_options);
        var entity = new EfEquipment
        {
            InventoryNumber = equipment.InventoryNumber,
            Name = equipment.Name,
            Department = equipment.Department,
            IsDeleted = false
        };
        db.Equipment.Add(entity);
        await db.SaveChangesAsync();
        return entity.Id;
    }

    public async Task SoftDeleteAsync(int id)
    {
        await using var db = new HelpDeskDbContext(_options);
        var entity = await db.Equipment.FirstOrDefaultAsync(x => x.Id == id);
        if (entity != null)
        {
            entity.IsDeleted = true;
            await db.SaveChangesAsync();
        }
    }
}

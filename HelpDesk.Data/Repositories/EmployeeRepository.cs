using HelpDesk.Core.Interfaces;
using HelpDesk.Data.EfModels;
using Microsoft.EntityFrameworkCore;
using CoreEmployee = HelpDesk.Core.Models.Employee;
using EfEmployee = HelpDesk.Data.EfModels.Employee;

namespace HelpDesk.Data.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly DbContextOptions<HelpDeskDbContext> _options;

    public EmployeeRepository(DbContextOptions<HelpDeskDbContext> options)
    {
        _options = options;
    }

    public async Task<CoreEmployee?> GetByIdAsync(int id)
    {
        await using var db = new HelpDeskDbContext(_options);
        var e = await db.Employees.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (e == null) return null;

        return new CoreEmployee
        {
            Id = e.Id,
            FullName = e.FullName,
            Position = e.Position,
            Department = e.Department,
            IsDeleted = e.IsDeleted
        };
    }

    public async Task<List<CoreEmployee>> GetAllActiveAsync()
    {
        await using var db = new HelpDeskDbContext(_options);
        return await db.Employees
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.FullName)
            .Select(e => new CoreEmployee
            {
                Id = e.Id,
                FullName = e.FullName,
                Position = e.Position,
                Department = e.Department,
                IsDeleted = e.IsDeleted
            })
            .ToListAsync();
    }

    public async Task<int> CreateAsync(CoreEmployee employee)
    {
        await using var db = new HelpDeskDbContext(_options);
        var entity = new EfEmployee
        {
            FullName = employee.FullName,
            Position = employee.Position,
            Department = employee.Department,
            IsDeleted = false
        };
        db.Employees.Add(entity);
        await db.SaveChangesAsync();
        return entity.Id;
    }

    public async Task SoftDeleteAsync(int id)
    {
        await using var db = new HelpDeskDbContext(_options);
        var entity = await db.Employees.FirstOrDefaultAsync(x => x.Id == id);
        if (entity != null)
        {
            entity.IsDeleted = true;
            await db.SaveChangesAsync();
        }
    }
}

using HelpDesk.Core.Enums;
using HelpDesk.Core.Interfaces;
using HelpDesk.Data.EfModels;
using Microsoft.EntityFrameworkCore;
using CoreTicket = HelpDesk.Core.Models.Ticket;
using CoreHistory = HelpDesk.Core.Models.TicketStatusHistory;
using EfTicket = HelpDesk.Data.EfModels.Ticket;
using EfHistory = HelpDesk.Data.EfModels.TicketStatusHistory;

namespace HelpDesk.Data.Repositories;

public class TicketRepository : ITicketRepository
{
    private readonly DbContextOptions<HelpDeskDbContext> _options;

    public TicketRepository(DbContextOptions<HelpDeskDbContext> options)
    {
        _options = options;
    }

    public async Task<CoreTicket?> GetByIdAsync(int id)
    {
        await using var db = new HelpDeskDbContext(_options);
        var t = await db.Tickets.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return t == null ? null : MapToCore(t);
    }

    public async Task<List<CoreTicket>> GetAllAsync(
        TicketStatus? status = null,
        Priority? priority = null,
        int? assigneeId = null,
        DateTimeOffset? from = null,
        DateTimeOffset? to = null)
    {
        await using var db = new HelpDeskDbContext(_options);
        var query = db.Tickets.AsNoTracking().AsQueryable();

        if (status.HasValue)
            query = query.Where(t => t.Status == status.Value.ToString());

        if (priority.HasValue)
            query = query.Where(t => t.Priority == priority.Value.ToString());

        if (assigneeId.HasValue)
            query = query.Where(t => t.AssigneeId == assigneeId.Value);

        if (from.HasValue)
            query = query.Where(t => t.CreatedAt >= from.Value.UtcDateTime);

        if (to.HasValue)
            query = query.Where(t => t.CreatedAt <= to.Value.UtcDateTime);

        var list = await query.OrderByDescending(t => t.CreatedAt).ToListAsync();
        return list.Select(MapToCore).ToList();
    }

    public async Task<int> CreateAsync(CoreTicket ticket)
    {
        await using var db = new HelpDeskDbContext(_options);
        await using var tx = await db.Database.BeginTransactionAsync();

        try
        {
            var entity = new EfTicket
            {
                AuthorId = ticket.AuthorId,
                EquipmentId = ticket.EquipmentId,
                AssigneeId = ticket.AssigneeId,
                Title = ticket.Title,
                Description = ticket.Description,
                Priority = ticket.Priority.ToString(),
                Status = ticket.Status.ToString(),
                CreatedAt = ticket.CreatedAt.UtcDateTime
            };

            db.Tickets.Add(entity);
            await db.SaveChangesAsync();

            db.TicketStatusHistories.Add(new EfHistory
            {
                TicketId = entity.Id,
                FromStatus = null,
                ToStatus = ticket.Status.ToString(),
                ChangedAt = ticket.CreatedAt.UtcDateTime,
                ChangedById = ticket.AuthorId
            });

            await db.SaveChangesAsync();
            await tx.CommitAsync();
            return entity.Id;
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }

    public async Task UpdateAsync(CoreTicket ticket)
    {
        await using var db = new HelpDeskDbContext(_options);
        var entity = await db.Tickets.FirstOrDefaultAsync(x => x.Id == ticket.Id);
        if (entity == null) return;

        entity.AssigneeId = ticket.AssigneeId;
        entity.Title = ticket.Title;
        entity.Description = ticket.Description;
        entity.Priority = ticket.Priority.ToString();

        await db.SaveChangesAsync();
    }

    public async Task ChangeStatusWithHistoryAsync(CoreTicket ticket, CoreHistory historyEntry)
    {
        await using var db = new HelpDeskDbContext(_options);
        await using var tx = await db.Database.BeginTransactionAsync();

        try
        {
            var entity = await db.Tickets.FirstOrDefaultAsync(x => x.Id == ticket.Id);
            if (entity != null)
            {
                entity.Status = ticket.Status.ToString();
            }

            db.TicketStatusHistories.Add(new EfHistory
            {
                TicketId = historyEntry.TicketId,
                FromStatus = historyEntry.FromStatus?.ToString(),
                ToStatus = historyEntry.ToStatus.ToString(),
                ChangedAt = historyEntry.ChangedAt.UtcDateTime,
                ChangedById = historyEntry.ChangedById
            });

            await db.SaveChangesAsync();
            await tx.CommitAsync();
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }

    public async Task<List<CoreHistory>> GetHistoryByTicketIdAsync(int ticketId)
    {
        await using var db = new HelpDeskDbContext(_options);
        var list = await db.TicketStatusHistories
            .AsNoTracking()
            .Where(h => h.TicketId == ticketId)
            .OrderBy(h => h.ChangedAt)
            .ToListAsync();

        return list.Select(h => new CoreHistory
        {
            Id = h.Id,
            TicketId = h.TicketId,
            FromStatus = string.IsNullOrEmpty(h.FromStatus) ? null : Enum.Parse<TicketStatus>(h.FromStatus),
            ToStatus = Enum.Parse<TicketStatus>(h.ToStatus),
            ChangedAt = new DateTimeOffset(DateTime.SpecifyKind(h.ChangedAt, DateTimeKind.Utc)),
            ChangedById = h.ChangedById
        }).ToList();
    }

    public async Task<bool> HasActiveTicketsByAssigneeAsync(int employeeId)
    {
        await using var db = new HelpDeskDbContext(_options);
        return await db.Tickets.AnyAsync(t =>
            t.AssigneeId == employeeId &&
            (t.Status == "New" || t.Status == "InProgress"));
    }

    private static CoreTicket MapToCore(EfTicket t)
    {
        return new CoreTicket
        {
            Id = t.Id,
            AuthorId = t.AuthorId,
            EquipmentId = t.EquipmentId,
            AssigneeId = t.AssigneeId,
            Title = t.Title,
            Description = t.Description,
            Priority = Enum.Parse<Priority>(t.Priority),
            Status = Enum.Parse<TicketStatus>(t.Status),
            CreatedAt = new DateTimeOffset(DateTime.SpecifyKind(t.CreatedAt, DateTimeKind.Utc))
        };
    }
}

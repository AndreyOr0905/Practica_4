using System;
using System.Collections.Generic;
using System.Text;
using HelpDesk.Core.Enums;
using HelpDesk.Core.Interfaces;
using HelpDesk.Core.Models;

namespace HelpDesk.Tests.Unit.Fakes
{
    public class FakeTicketRepository : ITicketRepository
    {
        public List<Ticket> Tickets { get; } = new();
        public List<TicketStatusHistory> History { get; } = new();

        public Task<Ticket?> GetByIdAsync(int id) =>
            Task.FromResult(Tickets.FirstOrDefault(t => t.Id == id));

        public Task<List<Ticket>> GetAllAsync(
            TicketStatus? status = null,
            Priority? priority = null,
            int? assigneeId = null,
            DateTimeOffset? from = null,
            DateTimeOffset? to = null)
        {
            var query = Tickets.AsEnumerable();
            if (status.HasValue) query = query.Where(t => t.Status == status.Value);
            if (priority.HasValue) query = query.Where(t => t.Priority == priority.Value);
            if (assigneeId.HasValue) query = query.Where(t => t.AssigneeId == assigneeId.Value);
            if (from.HasValue) query = query.Where(t => t.CreatedAt >= from.Value);
            if (to.HasValue) query = query.Where(t => t.CreatedAt <= to.Value);
            return Task.FromResult(query.ToList());
        }

        public Task<int> CreateAsync(Ticket ticket)
        {
            ticket.Id = Tickets.Count == 0 ? 1 : Tickets.Max(t => t.Id) + 1;
            Tickets.Add(ticket);
            History.Add(new TicketStatusHistory
            {
                TicketId = ticket.Id,
                FromStatus = null,
                ToStatus = ticket.Status,
                ChangedAt = ticket.CreatedAt,
                ChangedById = ticket.AuthorId
            });
            return Task.FromResult(ticket.Id);
        }

        public Task UpdateAsync(Ticket ticket)
        {
            var existing = Tickets.FirstOrDefault(t => t.Id == ticket.Id);
            if (existing != null)
            {
                existing.AssigneeId = ticket.AssigneeId;
                existing.Title = ticket.Title;
                existing.Description = ticket.Description;
                existing.Priority = ticket.Priority;
            }
            return Task.CompletedTask;
        }

        public Task ChangeStatusWithHistoryAsync(Ticket ticket, TicketStatusHistory historyEntry)
        {
            var existing = Tickets.FirstOrDefault(t => t.Id == ticket.Id);
            if (existing != null) existing.Status = ticket.Status;
            History.Add(historyEntry);
            return Task.CompletedTask;
        }

        public Task<List<TicketStatusHistory>> GetHistoryByTicketIdAsync(int ticketId) =>
            Task.FromResult(History.Where(h => h.TicketId == ticketId).ToList());

        public Task<bool> HasActiveTicketsByAssigneeAsync(int employeeId) =>
            Task.FromResult(Tickets.Any(t =>
                t.AssigneeId == employeeId &&
                (t.Status == TicketStatus.New || t.Status == TicketStatus.InProgress)));
    }
}

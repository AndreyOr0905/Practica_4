using System;
using System.Collections.Generic;
using System.Text;
using HelpDesk.Core.Enums;
using HelpDesk.Core.Models;
namespace HelpDesk.Core.Interfaces
{
    public interface ITicketRepository
    {
        Task<Ticket?> GetByIdAsync(int id);
        Task<List<Ticket>> GetAllAsync(TicketStatus? status = null, Priority? priority = null, int? assigneeId = null, DateTimeOffset? from = null, DateTimeOffset? to = null);
        Task<int> CreateAsync(Ticket ticket);
        Task UpdateAsync(Ticket ticket);
        Task ChangeStatusWithHistoryAsync(Ticket ticket, TicketStatusHistory historyEntry);
        Task<List<TicketStatusHistory>> GetHistoryByTicketIdAsync(int ticketId);
        Task<bool> HasActiveTicketsByAssigneeAsync(int employeeId);
    }
}

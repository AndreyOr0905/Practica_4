using System;
using System.Collections.Generic;
using System.Text;
using HelpDesk.Core.Enums;
using HelpDesk.Core.Exceptions;
using HelpDesk.Core.Interfaces;
using HelpDesk.Core.Models;

namespace HelpDesk.Core.Services
{
    public class TicketService
    {
        private readonly ITicketRepository _ticketRepo;
        private readonly IEmployeeRepository _employeeRepo;
        private readonly IEquipmentRepository _equipmentRepo;

        public TicketService(
            ITicketRepository ticketRepo,
            IEmployeeRepository employeeRepo,
            IEquipmentRepository equipmentRepo)
        {
            _ticketRepo = ticketRepo;
            _employeeRepo = employeeRepo;
            _equipmentRepo = equipmentRepo;
        }

        public async Task<int> CreateTicketAsync(int authorId, int equipmentId, string title, string description, Priority priority)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new DomainException("Заголовок заявки не может быть пустым.");

            if (title.Length > 200)
                throw new DomainException("Длина заголовка не должна превышать 200 символов.");

            if (description != null && description.Length > 2000)
                throw new DomainException("Длина описания не должна превышать 2000 символов.");

            var author = await _employeeRepo.GetByIdAsync(authorId);
            if (author == null || author.IsDeleted)
                throw new NotFoundException($"Автор с ID {authorId} не найден или удалён.");

            var equipment = await _equipmentRepo.GetByIdAsync(equipmentId);
            if (equipment == null || equipment.IsDeleted)
                throw new NotFoundException($"Оборудование с ID {equipmentId} не найдено или удалено.");

            var ticket = new Ticket
            {
                AuthorId = authorId,
                EquipmentId = equipmentId,
                Title = title.Trim(),
                Description = description ?? string.Empty,
                Priority = priority,
                Status = TicketStatus.New,
                CreatedAt = DateTimeOffset.UtcNow
            };

            return await _ticketRepo.CreateAsync(ticket);
        }

        public async Task AssignExecutorAsync(int ticketId, int executorId)
        {
            var ticket = await _ticketRepo.GetByIdAsync(ticketId)
                ?? throw new NotFoundException($"Заявка с ID {ticketId} не найдена.");

            if (ticket.Status is TicketStatus.Closed or TicketStatus.Cancelled)
                throw new DomainException("Нельзя назначить исполнителя на закрытую или отменённую заявку.");

            var executor = await _employeeRepo.GetByIdAsync(executorId);
            if (executor == null || executor.IsDeleted)
                throw new DomainException($"Исполнитель с ID {executorId} не найден или удалён.");

            ticket.AssigneeId = executorId;
            await _ticketRepo.UpdateAsync(ticket);
        }

        public async Task ChangeStatusAsync(int ticketId, TicketStatus newStatus, int changedById)
        {
            var ticket = await _ticketRepo.GetByIdAsync(ticketId)
                ?? throw new NotFoundException($"Заявка с ID {ticketId} не найдена.");

            if (!StatusTransitionValidator.IsTransitionAllowed(ticket.Status, newStatus))
                throw new DomainException($"Переход из статуса '{ticket.Status}' в '{newStatus}' запрещён.");

            if (newStatus == TicketStatus.InProgress && ticket.AssigneeId == null)
                throw new DomainException("Нельзя перевести заявку в работу без назначенного исполнителя.");

            var historyEntry = new TicketStatusHistory
            {
                TicketId = ticket.Id,
                FromStatus = ticket.Status,
                ToStatus = newStatus,
                ChangedAt = DateTimeOffset.UtcNow,
                ChangedById = changedById
            };

            ticket.Status = newStatus;
            await _ticketRepo.ChangeStatusWithHistoryAsync(ticket, historyEntry);
        }

        public async Task<List<Ticket>> GetOverdueTicketsAsync(DateTimeOffset? currentTime = null)
        {
            var now = currentTime ?? DateTimeOffset.UtcNow;
            var allTickets = await _ticketRepo.GetAllAsync();

            return allTickets
                .Where(t => t.Status is TicketStatus.New or TicketStatus.InProgress)
                .Where(t => now > SlaCalculator.CalculateSlaDeadline(t.CreatedAt, t.Priority))
                .ToList();
        }

        public async Task SoftDeleteEmployeeAsync(int employeeId)
        {
            var employee = await _employeeRepo.GetByIdAsync(employeeId)
                ?? throw new NotFoundException($"Сотрудник с ID {employeeId} не найден.");

            if (employee.IsDeleted)
                return;

            bool hasActive = await _ticketRepo.HasActiveTicketsByAssigneeAsync(employeeId);
            if (hasActive)
                throw new DomainException("Нельзя удалить сотрудника, пока на нём есть активные заявки (New или InProgress).");

            await _employeeRepo.SoftDeleteAsync(employeeId);
        }

        public async Task<TimeSpan> GetTotalInProgressDurationAsync(int ticketId, DateTimeOffset? currentTime = null)
        {
            var history = await _ticketRepo.GetHistoryByTicketIdAsync(ticketId);
            var ordered = history.OrderBy(h => h.ChangedAt).ToList();

            TimeSpan total = TimeSpan.Zero;
            DateTimeOffset? enteredInProgress = null;

            foreach (var entry in ordered)
            {
                if (entry.ToStatus == TicketStatus.InProgress)
                {
                    enteredInProgress = entry.ChangedAt;
                }
                else if (enteredInProgress.HasValue)
                {
                    total += entry.ChangedAt - enteredInProgress.Value;
                    enteredInProgress = null;
                }
            }

            if (enteredInProgress.HasValue)
            {
                var now = currentTime ?? DateTimeOffset.UtcNow;
                total += now - enteredInProgress.Value;
            }

            return total;
        }
    }
}
